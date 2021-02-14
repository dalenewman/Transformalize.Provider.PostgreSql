REM nuget pack Transformalize.Provider.PostgreSql.nuspec -OutputDirectory "c:\temp\modules"
REM nuget pack Transformalize.Provider.PostgreSql.Autofac.nuspec -OutputDirectory "c:\temp\modules"

nuget push "c:\temp\modules\Transformalize.Provider.PostgreSql.0.8.35-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json
nuget push "c:\temp\modules\Transformalize.Provider.PostgreSql.Autofac.0.8.35-beta.nupkg" -source https://www.myget.org/F/transformalize/api/v3/index.json






