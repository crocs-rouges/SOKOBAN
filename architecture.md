début de la réflextion sur l'architecture du projet 



prérequis du cahier des charges.



Scripts/
├── Managers/           # Singletons de gestion globale (Main, Grid, Sound)
├── Entities/           # Objets dynamiques du jeu
│   ├── Base/           # Classes abstraites (Movable)
│   ├── Player/         # Logique du joueur
│   └── Box/            # Objets à pousser (Dice, Caisse)
├── Environment/        # Éléments statiques ou triggers (Wall, FinishZone, CasinoCase)
├── UI/                 # HUD, Menus, Popups
└── Utils/              # Outils statiques et extensions