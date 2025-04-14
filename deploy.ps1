if (!(Test-Path -Path "./certs")) {
    mkdir -p certs
}
dotnet dev-certs https -ep ./certs/etuds.pfx -p "etuds"
Set-Location -Path "EtUdS.Server"
dotnet publish -c Release -o ./publish
Set-Location ..
docker compose up --build -d

