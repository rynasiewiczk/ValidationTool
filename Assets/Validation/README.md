# Validation Tool - validating Unity assets
Validation Tool is a plugin build for Unity3D that aims to help developers to keep their projects free from bugs related to invalid references and data.

## About Validation tool

### What it does
At its core, the tool checks all the scenes, prefabs, and Scriptable Objects for missing references, and then logs all of them in the console. It allows spotting unlinked references during both production and live-ops time.
The tool also gives you a way to create your own methods that can be fired alongside the core validations to make sure the data is structured in the way you expect it to be.

### Why to use it
While it's not a big issue to keep track of your project within a small team, it grows to a significant challenge along with the team's size. 
When you have dozens of engineers, artists, designers, and other people providing their work to the main branch, it gets impossible for humans to check all this content before it gets in. And even though it's the person's responsibility to check the content before pushing it forward, we're all only human and we all make simple mistakes once in a while.

### How to use it
To run a validation process, simply select `Tools -> ValidationTool -> Validate Everything`. This way the tool will go through all the scenes, prefabs, and scriptable objects, starting from a folder specified in the `ValidationConfig` asset. By default, the starting path is simply `Assets/`, but I find it a good practice to keep all of your project assets (except for plugins, frameworks, etc.) inside a folder inside it: `Assets/YourProjectAssets/`, to keep the hierarchy cleaner.
If you want to add your own method to a validation process, you should add a class that inherits from `BaseValidation`, and decorate your overridden `Validate` method with `ValidateMethodAttribute`. This way the method will be included in the process of validating your project.

### How to use it - CD/CI
It's a common scenario: add to a project a tool that is supposed to help you with the quality, use it for the first two weeks, and then just start skipping it because you're late/tired/don't care. To fully utilize the tool, it's recommended to add it to your CD/CI pipeline (if you have it or you can have it). Most popular code hosting platforms (like GitHub, GitLab) let you create one.

You can check [GameCI](https://game.ci/docs) page for guidelines on how to set the pipeline. There is also a [good tutorial for GitLab](https://dzone.com/articles/the-road-to-continuous-integration-in-unity) on how to set it up.

With these pipelines in place, you can validate the state of the project on a branch when a merge request is created and allow the merge only when to branch passes the validation.


### Validation exceptions
In some cases, you expect in your project an inspector field to be left not assigned. In that case, you can decorate the field with `OptionalObjectFieldAttribute` to keep it out of the process.

There are also components with empty fields within the engine itself, as well as it's probable that they will be empty in some plugins or frameworks that you're using. In that case, there are three ways to handle that:
1. Out of validation namespaces - you can enter names of namespaces that you don't want to validate. By default `UnityEngine` and `TMPro` namespaces are set in here because there are components in there empty by default and as an engine user you can do nothing about it. 
You can specify the namespaces in the `ValidationConfig` asset.
2. Out of validation paths - you can enter paths to folders that you want to keep out of the validation. A common case for that is a `Plugin` folder inside of the main `Assets` folder. You will find this in the `ValidationConfig` asset as well.
3. Out of validation Component type names - if you want some components to be kept out of the process, you can set them in here. As before, they can be set in the `ValidationConfig` asset.

You can also enter scene names for the ones that you want to keep out of the process if it's something you want.
Another situation where you may want to have an empty reference is with prefabs. It's common to build a base prefab with empty fields that are filled only by this prefab's variants. To keep these assets out of validation, you can put the prefabs inside a folder named `Abstract`. This way the tool will know to leave the prefab in peace.

## Unity version
The tool is supported for Unity 2019.4 and newer versions. 