# Tazos Kanto

Juego móvil 3D de tazos con los 151 Pokémon originales. Tiras un tazo contra una pila y
te quedas con todos los que consigas dar la vuelta.

- **Documento de diseño completo**: [`docs/DESIGN.md`](docs/DESIGN.md) — concepto, controles
  táctiles, física, efecto palanca, tecnología, arquitectura, roadmap y Fase 0.
- **Guía de primera prueba**: [`docs/PLAYTEST.md`](docs/PLAYTEST.md) — instalación, gesto
  de lanzamiento y criterios para valorar el prototipo Unity.
- **Prototipo principal (Fase 0)**: [`unity/`](unity/) — Unity 6, todo por código, sin escenas.
- **Prototipo web secundario**: [`web/index.html`](web/index.html) — herramienta de prueba rápida;
  ya no es la referencia de producto ni se garantiza paridad física con Unity.

## Prioridad técnica

**Unity es la fuente de verdad para el producto móvil.** Toda mecánica nueva y todo ajuste de
balance se implementan y validan primero en `unity/`. La versión web queda como una demo rápida
para probar un gesto sin instalar el motor; no debe recibir funcionalidad de producto.

## Estado

Fase 0: prototipo físico. Mesa, pila de 10 tazos + 6 sueltos, lanzamiento con los cinco
canales de control táctil, física completa de discos, palanca y vuelco en el canto (ambos
calibrables), detección de volteo, combos y contador de Pokédex. Sin tienda, monedas,
cuentas ni multijugador: eso no arregla un lanzamiento que no se sienta bien.

## Prototipo web (secundario)

`web/index.html` es un único fichero para comprobar rápidamente el gesto en un navegador
móvil. No representa la ruta de entrega ni sustituye las pruebas en una build de Unity.

## Uso interno e IP

Este repositorio es un prototipo de uso personal e interno. Pokémon, sus nombres y diseños son
propiedad de sus respectivos titulares; no hay licencia para distribuir, publicar, monetizar ni
asociar este proyecto con esa propiedad intelectual. Mantén el repositorio y las builds privadas,
y no añadas arte, logotipos ni contenido oficial. Si el proyecto va a salir de ese ámbito, debe
convertirse a una IP original o contar con permiso de los titulares.

## Cómo ejecutar el prototipo de Unity

1. Unity Hub → **New project → 3D (URP)** con **Unity 6.0 LTS**.
2. Copia `unity/Assets/Scripts/` dentro de la carpeta `Assets` del proyecto nuevo.
3. `Edit → Project Settings → Player → Active Input Handling` = **Both** (o *Input Manager
   (Old)*): el prototipo usa la API de entrada clásica para poder iterar sin fricción.
4. Play. No hay escena que abrir: `Bootstrap` construye mesa, tazos, cámara, luces y HUD
   al arrancar en cualquier escena, incluso vacía.

En el editor se juega con el ratón (arrastrar y soltar). En el móvil, con el pulgar.

### Compilar para iPhone desde Windows

`File → Build Settings → iOS`, IL2CPP + ARM64, y Unity genera un proyecto Xcode. Firmarlo
y subirlo a TestFlight exige un Mac o un servicio de build en la nube: es requisito de
Apple, no del motor. Todo el desarrollo e iteración se hace en Windows.

## Controles

Un solo pulgar, gesto de tirachinas: arrastra hacia atrás y suelta.

| Canal | Gesto |
|---|---|
| Dirección | Ángulo del arrastre (se lanza al contrario) |
| Potencia | Longitud del arrastre |
| Ángulo de salida | Componente vertical del *flick* al soltar |
| Inclinación | Componente lateral de ese mismo flick |
| Spin | Curvatura del recorrido del dedo |

Botones del HUD: **Cambiar tazo** (Snorlax, Geodude, Scyther, Pikachu, Mewtwo) y **Física**,
que abre los deslizadores para tocar en vivo el impulso de palanca, la velocidad máxima,
el spin y la inclinación. Ese panel es la herramienta principal de esta fase.

## Dónde tocar

| Quiero cambiar… | Fichero |
|---|---|
| Sensación general (velocidades, palanca, nº de tiros) | `GameTuning.cs` |
| Cómo se comporta cada tipo de tazo | `TazoProfile.cs` |
| Silueta del disco: bisel y domo (clave de la palanca) | `TazoMeshFactory.cs` |
| Lectura del gesto táctil | `ThrowController.cs` |
| Palanca y aerodinámica | `Tazo.cs` |
| Reparto, turnos, combos | `GameSession.cs` |
