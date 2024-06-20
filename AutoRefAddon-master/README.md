# AutoRefAddon

This add-on adds a new function to the MacroEditor of Citavi to include directives of custom referenced assemblies. Everytime the user save the macro code the addon adds comments to the code file. When the user reopen the macro the comments will be removed and the assembly will be add to the internal macro editor compiler service.

## Format of comment

```csharp

// autoref "[ASSEMBLYPATH]"

```

## Releases

The compiled library can be found under [releases](./../../releases) as an archive.

## Disclaimer

>There are no support claims by the company **Swiss Academic Software GmbH**, the provider of **Citavi** or other liability claims for problems or data loss. Any use is at your own risk. All rights to the name **Citavi** and any logos used are owned by **Swiss Academic Software GmbH**.

## License

This project is licensed under the [MIT](LICENSE) License
