## Setup du Projet ÉtUdS
### Pré-requis 
L'application nécessite NPM (Node JS) et Angular pour être développée. Le site web est accessible à tous dès l'instant où il est déployé.

## Déploiement de l'application
_Les étapes suivantes doivent être exécutés sur la machine qui sera responsable d'héberger le site._

### Préparation de l'application
Il est conseillé d'utiliser le fichier `deploy.ps1` pour exécuter les instructions suivantes (à la place de celles-ci), bien qu'elles puissent être faites manuellement.

Créer le répertoire `certs` à la racine du projet et ajouter le certificat https en faisant `dotnet dev-certs https -ep ./certs/etuds.pfx -p "etuds"`

Afin de générer le `.dll` nécessaire au conteneur pour rouler le backend, la commande suivante doit être utilisé dans le répertoire `Serveur` de l'application : `dotnet publish -c Release -o ./publish`

### Accès à l'application
Au répertoire racine de l'application, utiliser la commande `docker compose up --build -d`

- Le site web est désormais accessible à l'adresse : https://VOTRE-IP (ip privée)

### Préparation de la base de donnée
Pour que l'application fonctionne, celle-ci doit avoir des données. Il est nécessaire de rouler les script `.sql` suivant pour 1. Créer les tables et 2. Simuler des données : 
- `CreateTables.sql`
- `TestData.sql`

_Notez que ces instructions ne font pas parties du `deploy.ps1` car on ne veut pas réexecuter ces scripts à chaque déploiement (seulement une fois initialement)._

## Développement de l'application
Pour executer l'application en local (accessible sur votre ordinateur uniquement) : `dotnet run --launch-profile EtUdS`

L'application (site web) est maintenant disponible à l'adresse : https://localhost:4200

- Le swagger (utile pour tester l'api backend) est disponible à l'adresse : https://localhost:7055/swagger/index.html
- Tous les "calls" au back-end passe sur une application déployé, vous devez donc avoir le Docker du projet de démarré pour que ceux-ci fonctionnent (voir `Déploiement de l'application`).