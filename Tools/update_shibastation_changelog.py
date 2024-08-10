import os
import yaml
from datetime import datetime

# Define paths
script_dir = os.path.dirname(os.path.abspath(__file__))
parts_dir = os.path.join(script_dir, '../Resources/Changelog/Parts')
changelog_dir = os.path.join(script_dir, '../Resources/Changelog')

def load_yaml_file(file_path):
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            return yaml.safe_load(f)
    except Exception as e:
        print(f"Error loading YAML file {file_path}: {e}")
        return None

def save_yaml_file(data, file_path):
    try:
        with open(file_path, 'w', encoding='utf-8') as f:
            yaml.dump(data, f, default_flow_style=False, allow_unicode=True)
    except Exception as e:
        print(f"Error saving YAML file {file_path}: {e}")

def get_next_id(entries):
    try:
        ids = [entry.get('id') for entry in entries if isinstance(entry, dict) and 'id' in entry]
        return max(ids) + 1 if ids else 1
    except Exception as e:
        print(f"Error getting next ID: {e}")
        return 1

def update_changelog(part, changelog_path, next_id, part_mod_time):
    try:
        changelog = load_yaml_file(changelog_path)
        if not changelog or 'Entries' not in changelog or not isinstance(changelog['Entries'], list):
            print(f"Unexpected format in changelog file {changelog_path}")
            return False

        part_entry = {
            'author': part.get('author', 'Unknown'),
            'changes': part.get('changes', []),
            'id': next_id,
            'time': part_mod_time
        }
        changelog['Entries'].insert(0, part_entry)
        save_yaml_file(changelog, changelog_path)
        return True
    except Exception as e:
        print(f"Error updating changelog {changelog_path}: {e}")
        return False

def main():
    parts_files = [f for f in os.listdir(parts_dir) if f.endswith('.yml')]
    log_entries = []

    for part_file in parts_files:
        part_path = os.path.join(parts_dir, part_file)
        part = load_yaml_file(part_path)

        if part is None:
            log_entries.append(f"Failed to parse: {part_file}")
            continue

        category = part.get('category', 'ShibaStation')
        changelog_path = os.path.join(changelog_dir, f"{category}.yml")

        if not os.path.exists(changelog_path):
            log_entries.append(f"Changelog file not found for category '{category}': {part_file}")
            continue

        changelog = load_yaml_file(changelog_path)
        if changelog is None or 'Entries' not in changelog or not isinstance(changelog['Entries'], list):
            log_entries.append(f"Unexpected format in changelog file: {changelog_path}")
            continue

        next_id = get_next_id(changelog['Entries'])

        part_mod_time = datetime.fromtimestamp(os.path.getmtime(part_path)).isoformat()

        if update_changelog(part, changelog_path, next_id, part_mod_time):
            os.remove(part_path)
        else:
            log_entries.append(f"Failed to update changelog: {part_file}")

if __name__ == '__main__':
    main()
