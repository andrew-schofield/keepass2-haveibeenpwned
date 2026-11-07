#!/usr/bin/env python3
"""Regression test: PLGX packaging must not reference files outside the project root.

KeePass >= 2.62 validates every path stored in a .plgx archive and refuses to
load a plugin whose project references (Compile / EmbeddedResource / None /
Content / Resource / Page / ApplicationDefinition) or local assembly HintPaths
escape the project directory (e.g. "..\\keepass2-developerextensions\\...").

KeePass builds the PLGX from the .csproj, so the project file itself must only
contain paths that stay inside the project root. This test parses the project
file and fails on any escaping path.

Run: python3 -m unittest discover -s tests -v
"""

import os
import re
import unittest
import xml.etree.ElementTree as ET

REPO_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PROJECT_DIR = os.path.join(REPO_ROOT, "HaveIBeenPwned")
PROJECT_FILE = os.path.join(PROJECT_DIR, "HaveIBeenPwned.csproj")

MSBUILD_NS = "{http://schemas.microsoft.com/developer/msbuild/2003}"

# Item groups whose Include paths end up inside the .plgx archive.
PACKAGED_ITEM_TYPES = (
    "Compile",
    "EmbeddedResource",
    "None",
    "Content",
    "Resource",
    "Page",
    "ApplicationDefinition",
)


def _localname(tag):
    return tag.split("}", 1)[1] if "}" in tag else tag


def _is_msbuild_property(path):
    """True for MSBuild property/function expressions, not literal paths."""
    return "$(" in path or "@(" in path or "%(" in path


def _escapes_project_root(rel_path):
    """Return True if rel_path (project-relative, Windows or POSIX separators)
    resolves outside the project directory."""
    normalized = rel_path.replace("\\", "/")
    if normalized.startswith("/") or re.match(r"^[A-Za-z]:", normalized):
        return True
    depth = 0
    for part in normalized.split("/"):
        if part in ("", "."):
            continue
        if part == "..":
            depth -= 1
            if depth < 0:
                return True
        else:
            depth += 1
    return False


def _load_project():
    tree = ET.parse(PROJECT_FILE)
    return tree.getroot()


def _iter_items(root, item_type):
    for group in root:
        if _localname(group.tag) != "ItemGroup":
            continue
        for item in group:
            if _localname(item.tag) == item_type:
                yield item


def _include_of(item):
    return item.get("Include") or item.get("Update") or ""


class PlgxPathContainmentTest(unittest.TestCase):
    def test_project_file_exists(self):
        self.assertTrue(os.path.isfile(PROJECT_FILE), PROJECT_FILE)

    def test_packaged_items_stay_inside_project_root(self):
        root = _load_project()
        offenders = []
        for item_type in PACKAGED_ITEM_TYPES:
            for item in _iter_items(root, item_type):
                include = _include_of(item)
                if not include or _is_msbuild_property(include):
                    continue
                if _escapes_project_root(include):
                    offenders.append("%s Include=%s" % (item_type, include))
        self.assertEqual(
            [],
            offenders,
            "PLGX project references escape the project root "
            "(KeePass >= 2.62 rejects these):\n  " + "\n  ".join(offenders),
        )

    def test_packaged_items_exist_on_disk(self):
        root = _load_project()
        missing = []
        for item_type in PACKAGED_ITEM_TYPES:
            for item in _iter_items(root, item_type):
                include = _include_of(item)
                if not include or _is_msbuild_property(include):
                    continue
                if _escapes_project_root(include):
                    continue  # reported by the containment test
                full = os.path.join(PROJECT_DIR, include.replace("\\", "/"))
                if not os.path.isfile(full):
                    missing.append("%s Include=%s" % (item_type, include))
        self.assertEqual([], missing, "Missing packaged files:\n  " + "\n  ".join(missing))

    def test_local_assembly_hintpaths_stay_inside_project_root(self):
        """HintPaths that point at a local file (not an installed KeePass) must
        stay inside the project root; absolute install paths are ignored."""
        root = _load_project()
        offenders = []
        for item in _iter_items(root, "Reference"):
            hint = None
            for child in item:
                if _localname(child.tag) == "HintPath":
                    hint = (child.text or "").strip()
            if not hint or _is_msbuild_property(hint):
                continue
            if re.match(r"^[A-Za-z]:", hint) or hint.startswith("\\") or hint.startswith("/"):
                continue  # absolute path to an installed assembly (KeePass.exe)
            if _escapes_project_root(hint):
                offenders.append("Reference %s HintPath=%s" % (item.get("Include"), hint))
        self.assertEqual(
            [],
            offenders,
            "Local assembly HintPaths escape the project root:\n  " + "\n  ".join(offenders),
        )

    def test_no_relative_paths_escape_in_raw_project_text(self):
        """Belt-and-braces: no attribute value in the project file may contain a
        '..' segment, regardless of which item type it belongs to."""
        with open(PROJECT_FILE, "r", encoding="utf-8-sig") as handle:
            text = handle.read()
        offenders = []
        for match in re.finditer(r'(?:Include|Update|HintPath)\s*=\s*"([^"]*)"', text):
            value = match.group(1)
            if _is_msbuild_property(value):
                continue
            if re.match(r"^[A-Za-z]:", value) or value.startswith("\\") or value.startswith("/"):
                continue
            if ".." in value.replace("\\", "/").split("/"):
                offenders.append(value)
        self.assertEqual([], offenders, "Escaping paths in project file: %r" % (offenders,))


if __name__ == "__main__":
    unittest.main(verbosity=2)
