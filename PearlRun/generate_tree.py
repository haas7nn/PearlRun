import os

def generate_tree(path, prefix=""):
    tree = ""
    entries = sorted(os.listdir(path))
    entries = [e for e in entries if e not in ['Library', 'Temp', 'Obj', 'Build', '.vs', '.git']]
    for i, entry in enumerate(entries):
        full_path = os.path.join(path, entry)
        connector = "├── " if i < len(entries) - 1 else "└── "
        tree += f"{prefix}{connector}{entry}\n"
        if os.path.isdir(full_path):
            extension = "│   " if i < len(entries) - 1 else "    "
            tree += generate_tree(full_path, prefix + extension)
    return tree

if __name__ == "__main__":
    project_path = "."  # Change this to your Unity project folder path
    tree = generate_tree(project_path)
    with open("ProjectTree.md", "w", encoding="utf-8") as f:
        f.write("```\n" + tree + "```\n")
    print("Project tree generated in ProjectTree.md")