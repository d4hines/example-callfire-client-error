# Example Repository for callfire-api-client-csharp issue #55
This repo is to demonstrate the error in [this issue](https://github.com/CallFire/callfire-api-client-csharp/issues/55)

In test `Should_Send_Text_Broadcast` (in `CallFirePlugin.Tests.cs`, line 82), I'm getting the following error at runtime:

```bat
System.IO.FileLoadException : Could not load file or assembly 'callfire-api-client, Version=1.1.19.29318, Culture=neutral, PublicKeyToken=null' or one of its dependencies. A strongly-named assembly is required. (Exception from HRESULT: 0x80131044)
```

The weird thing is that, in that very same file, `SendVM` (line 97) works! (If you replace `"username"` and `"password"`)

The following command is running as post-build events in both projects:

```bat
xcopy "$(SolutionDir)packages\CallfireApiClient.1.1.19\lib\callfire-api-client.dll.config" "$(TargetDir)" /i /R /Y
```
