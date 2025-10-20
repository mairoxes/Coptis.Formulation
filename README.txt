README — Coptis.Formulation
 Architecture et choix techniques

Le projet Coptis.Formulation est basé sur une architecture DDD (Domain-Driven Design) afin de garantir une séparation claire entre les préoccupations métier, applicatives et techniques.
Ce choix a été motivé par la nécessité d’avoir :

une couche Domain indépendante et testable contenant uniquement la logique métier (entités, agrégats, valeur-objets) ;

une couche Application dédiée à la coordination des cas d’usage, avec des Services et DTOs ;

une couche Infrastructure gérant les détails techniques comme la persistance, les transactions et l’import automatique de fichiers.

Cette organisation rend le projet plus maintenable, facilite les tests unitaires et permet d’envisager des extensions futures sans impacter le cœur métier.

 État d’avancement

Les fonctions principales côté back-end (import, validation, calculs) sont opérationnelles.

La partie front-end (Blazor WebAssembly) est en cours : certaines pages comme Import et Formulas fonctionnent mais nécessitent encore du raffinement et de l’intégration complète (boutons, interactions API, gestion des erreurs).

Plusieurs composants restent à réorganiser, notamment dans la partie Application où certains services et validations pourraient être déplacés ou regroupés pour une meilleure cohérence.

Quelques configurations locales (chemins de fichiers, options d’import) sont encore codées en dur — elles devront être externalisées dans des fichiers de configuration ou injectées via des services IOptions.

 Composant d’import automatique

Le Hosted Service (AutoImportHostedService) permet d’écouter un dossier local et d’importer automatiquement les fichiers JSON déposés.
Bien que pratique pour un environnement de développement, ce mécanisme a certaines limites :

dépendance au système de fichiers local ;

difficulté à monitorer et à scaler dans un environnement cloud ;

gestion plus complexe des erreurs et redéploiements.

Hébergement : Hosted Service vs Azure Function vs Batch
Solution	Avantages	Inconvénients	Cas d’usage recommandé
Hosted Service (Worker dans ASP.NET)	Intégré dans l’application, simple à déployer, idéal pour tâches locales ou temporisées.	Couplé au cycle de vie de l’API, pas adapté au scale-out, pas optimal pour la tolérance aux pannes.	Développement local, petites tâches planifiées.
Azure Function	Faible coût, déclencheurs (Timer, Blob, Queue), monitoring intégré, haute disponibilité.	Moins de contrôle sur l’environnement, complexité de configuration CI/CD.	Traitement de fichiers déposés dans un Blob Storage, logique événementielle.
Azure Batch / Logic App	Gestion de gros volumes, planification avancée, orchestration.	Surdimensionné pour de petits traitements, configuration plus lourde.	Scénarios industriels avec traitements massifs et planifiés.