# Système Odyssey - Guide d'utilisation

## Vue d'ensemble

Le système Odyssey permet de créer un objet volant contrôlable dans Unity qui :
- Permet au joueur de monter dessus et de le contrôler
- Désactive les contrôles normaux du joueur quand il est sur l'Odyssey
- Empêche le joueur de passer à travers l'Odyssey pendant le mouvement
- Disparaît automatiquement après 5 secondes de contrôle avec effet de fade

## Composants

### 1. OdysseyController.cs
Gère la détection du joueur et les contrôles de l'Odyssey.

**Paramètres principaux :**
- `moveSpeed` : Vitesse de déplacement contrôlé (défaut: 10f)
- `acceleration` : Vitesse d'accélération (défaut: 5f)
- `deceleration` : Vitesse de décélération (défaut: 8f)
- `detectionRadius` : Rayon de détection du joueur (défaut: 2f)
- `useParentingMethod` : Méthode de mouvement recommandée (défaut: true)
- `controlTimeLimit` : Temps en secondes avant disparition (défaut: 5f)
- `fadeOutEffect` : Active l'effet de fade avant disparition (défaut: true)

### 3. OdysseyTestManager.cs (Optionnel)
Script de test pour valider le fonctionnement du système.

## Installation

### Étape 1 : Préparation de l'objet Odyssey
1. Créez un GameObject pour l'Odyssey
2. Ajoutez un **Collider2D** (BoxCollider2D ou CircleCollider2D)
3. Ajoutez un **Rigidbody2D** (sera configuré automatiquement en Kinematic)
4. Ajoutez le script **OdysseyController**

### Étape 2 : Configuration du joueur
Le joueur doit avoir :
- Le tag "Player"
- Un script **PlayerController** (du système de plateforme)
- Un **Rigidbody2D**
- Un **Collider2D**

## Configuration recommandée

### Paramètres OdysseyController
```csharp
moveSpeed = 8f;           // Vitesse confortable
acceleration = 6f;        // Réactivité rapide
deceleration = 10f;       // Arrêt rapide
detectionRadius = 1.5f;   // Ajuster selon la taille de l'Odyssey
useParentingMethod = true; // Méthode recommandée
controlTimeLimit = 5f;    // 5 secondes de contrôle
fadeOutEffect = true;     // Effet de disparition visuel
```

## Méthodes de mouvement du joueur

### Méthode Parentage (Recommandée)
- **Avantages :** Mouvement fluide, pas de conflits de collision
- **Utilisation :** Définir `useParentingMethod = true`
- **Comment ça marche :** Le joueur devient temporairement enfant de l'Odyssey

### Méthode Manuelle
- **Avantages :** Plus de contrôle fin, compatible avec systèmes complexes
- **Utilisation :** Définir `useParentingMethod = false`
- **Comment ça marche :** Déplace le joueur manuellement avec des méthodes physiques sûres

## Contrôles

### Contrôles du joueur sur l'Odyssey
- **Flèches/WASD :** Mouvement directionnel
- **Joystick :** Support complet des manettes

### Touches de test (OdysseyTestManager)
- **Y :** Affichage des informations du joueur
- **U :** Changement de méthode de mouvement
- **I :** Informations sur le timer de disparition
- **O :** Force la disparition immédiate

## Débogage

### Visualisation
- **Gizmos :** Cercle de détection visible dans l'éditeur
- **LineRenderer :** Cercle de détection optionnel en runtime
- **Logs :** Messages détaillés dans la console

### Messages de debug courants
```
"Joueur monté sur l'Odyssey" - Détection réussie
"Timer de disparition démarré - 5s avant disparition" - Timer activé
"ATTENTION: L'Odyssey va disparaître dans 1 seconde!" - Avertissement
"L'Odyssey disparaît après 5 secondes de contrôle!" - Disparition
"Joueur descendu de l'Odyssey" - Joueur quitté
"Ascension stoppée - Joueur en contrôle" - Système fonctionnel
```

## Résolution de problèmes

### Le joueur traverse l'Odyssey
- Vérifiez que `useParentingMethod = true`
- Ajustez `detectionRadius` si nécessaire
- Vérifiez que le joueur a bien le tag "Player"

### L'ascension ne s'arrête pas
- Vérifiez que les deux scripts communiquent (propriété `IsPlayerControlling`)
- Contrôlez que la détection du joueur fonctionne

### Contrôles non responsifs
- Vérifiez que `PlayerController.controlEnabled` est bien géré
- Testez avec différentes méthodes de mouvement

### L'Odyssey disparaît trop rapidement/lentement
- Ajustez `controlTimeLimit` dans les paramètres
- Désactivez `fadeOutEffect` si l'effet visuel gêne
- Vérifiez que le timer ne se relance pas en boucle

### L'effet de fade ne fonctionne pas
- Vérifiez qu'un `SpriteRenderer` est attaché à l'Odyssey
- Activez `fadeOutEffect = true`
- Assurez-vous que le Sprite a un canal alpha

### Le joueur reste bloqué après disparition
- Le système restaure automatiquement le joueur
- Vérifiez que `RestorePlayer()` est appelée
- Contrôlez les logs pour détecter les erreurs

## Performance

### Optimisations appliquées
- Détection du joueur limitée par rayon
- Gizmos de debug désactivés en runtime
- Mouvement physique optimisé avec `MovePosition()`
- Mise à jour conditionnelle des positions

### Paramètres de performance
- `detectionRadius` : Plus petit = meilleure performance
- Debug activé uniquement en mode éditeur
- Mouvements micro évités avec seuils minimum

## Extension

### Ajouter de nouvelles fonctionnalités
1. Héritez des classes existantes
2. Utilisez les propriétés publiques (`IsPlayerControlling`)
3. Écoutez les events Unity standard (`OnTriggerEnter2D`, etc.)

### Intégration avec d'autres systèmes
- Le système respecte le `PlayerController` existant
- Compatible avec les systèmes de particules et d'audio
- Extensible pour multiplayer avec modifications

## Notes techniques

### Système de collision
- Utilise `Physics2D.OverlapCircleAll()` pour la détection
- Méthode `Teleport()` pour les mouvements critiques
- `MovePosition()` pour les mouvements fluides

### Architecture
- Séparation claire des responsabilités
- Communication par propriétés publiques
- Système modulaire et extensible

---

**Version :** 1.0  
**Compatibilité :** Unity 2021.3+  
**Dépendances :** Système de plateforme existant (PlayerController, KinematicObject)
