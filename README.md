# POC Scriglov VR : test 1

Epitech Forward · test handtracking 1 · état : mars 2018

Preuve de concept en réalité mixte (Meta Quest, Unity 6) : un feu tricolore flotte devant l'utilisateur et réagit à sa main droite, suivie sans manette.

## Ce qui fonctionne

- **Feu tricolore** construit en primitives, placé devant l'utilisateur au lancement ([PlaceInFront.cs](Assets/Scripts/PlaceInFront.cs)), à hauteur d'écran.
- **Suivi de la main droite** (XR Hands) : score d'ouverture de 0 à 1, calculé pour chaque doigt ([HandOpenness.cs](Assets/Scripts/HandOpenness.cs)).
- **Commande du feu par la main** ([HandTrafficLightController.cs](Assets/Scripts/HandTrafficLightController.cs)) :
  - poing fermé : rouge
  - main mi-ouverte : orange
  - main ouverte : vert
  - pouce, index et auriculaire tendus, majeur et annulaire repliés : mode « Spider » (image et son)
- **Affichage de la main droite** : une sphère par articulation ([HandVisualizer.cs](Assets/Scripts/HandVisualizer.cs)).
- **Panneau de debug** sur la main : version, état du suivi, ouverture de chaque doigt, état du feu ([HandDebug.cs](Assets/Scripts/HandDebug.cs)).
- **Passthrough** : le feu s'affiche sur l'image réelle du casque.

## En attente

- **Implémentation de l'exercice** : le scénario d'exercice que l'utilisateur doit réaliser avec sa main n'est pas encore codé.
- **Interface** : menus, consignes et retours à l'utilisateur restent à concevoir. Le seul affichage actuel est le panneau de debug.

## TODO

- Ajouter les features du PLD (2018, 2019, 2020).

## Lancer le projet

- Unity 6000.6.7f1
- Packages : XR Hands, XR Interaction Toolkit, OpenXR, Meta OpenXR, AR Foundation
- Ouvrir la scène `Assets/Feu.unity`, puis faire un build Android et l'envoyer sur le Quest (Build And Run). Le suivi des mains doit être activé dans les réglages du casque.
