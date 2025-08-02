---
description: How to add or edit rules in our project
globs:
alwaysApply: false
---

# Cursor Rules Location

How to add new cursor rules to the project

1. Always place rule files in PROJECT_ROOT/rules/:

   ```
   ./rules/
   ├── your-rule-name.md
   ├── another-rule.md
   └── ...
   ```

2. Follow the naming convention:

   - Use kebab-case for filenames
   - Always use .md extension
   - Make names descriptive of the rule's purpose

3. Directory structure:

   ```
   PROJECT_ROOT/
   ├── rules/
   │   ├── your-rule-name.md
   │   └── ...
   └── ...
   ```

4. Never place rule files:

   - In the project root
   - In subdirectories outside ./rules
   - In any other location

5. Rules have the following structure:

````
---
description: Short description of the rule's purpose
globs: optional/path/pattern/**/*
alwaysApply: false
---
# Rule Title

Main content explaining the rule with markdown formatting.

1. Step-by-step instructions
2. Code examples
3. Guidelines

Example:

```typescript
// Good example
function goodExample() {
  // Implementation following guidelines
}

// Bad example
function badExample() {
  // Implementation not following guidelines
}
````

6. Add a reference to the rule in the main CLAUDE.md file that has a brief description and the file path (using claude's `@` syntax) like:

```markdown
## Rules Reference

Reference these rules as needed to get detailed guidance on various actions and operations.

- For guidance on writing rules, see [Writing Rules](./rules/writing-rules.md)
- For guidance on dealing with async operations, server state, and api calls, see [Async Operations](./rules/apis-and-async-state.md)
```
