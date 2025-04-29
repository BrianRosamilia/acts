## Running

Run the project in Visual Studio.  This will NPM install, build the frontend, build the backend, and run the project (will take a while on first run)

Test with `dotnet test` inside `api.Tests`

UI is here `https://localhost:7204/index.html`

If you run into issues getting the front end to because of the pre-build event that requires powershell, delete this from the project

```
  <Target Name="PreBuild" BeforeTargets="PreBuildEvent">
      <Exec Command="powershell -Command &quot;Set-Location -Path '../client'; Write-Host 'Current directory:' (Get-Location).Path; npm install; npm run build&quot;" />
  </Target>
``

And then `cd` to client and run `npm install` and `npm run build`

`John Doe` has some career data pre-populated.

## Considerations

* Prefer LINQ over dapper/raw sql. I converted all the raw SQL but there was some string interpolation susceptible to SQLI that would have needed to be parameterized. Additionally, there is now `context.Database.SqlQuery<T>` in the latest version of entity framework so Dapper can be replaced when raw SQL is desired.
* Could send additional logs from the commands. Felt like a bit overkill for this exercise and I don't have a feel for the logging level expected in the organization yet.
* Could have normalized and improved table structure, _however_, the rules specifically state to use a person's name as a primary key and that the data is updated by an external system (which *can't* be happening through the API since there is no AstronautDetail API endpoint). Additionally, if you hookup `SeedData`, you do get AstronautDetail data so I also assume this is simulating the external system.
* Normally I dont test against an in-memory DB, I make an IDBContext interface and use a FakeDbSet<T> that is capable of faking `SaveChanges`.