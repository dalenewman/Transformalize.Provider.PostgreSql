nuget pack Transformalize.Provider.PostgreSql.nuspec -OutputDirectory "c:\temp\modules"
nuget pack Transformalize.Provider.PostgreSql.Autofac.nuspec -OutputDirectory "c:\temp\modules"

nuget push "c:\temp\modules\Transformalize.Provider.PostgreSql.0.3.5-beta.nupkg" -source https://api.nuget.org/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.PostgreSql.Autofac.0.3.5-beta.nupkg" -source https://api.nuget.org/v3/index.json






