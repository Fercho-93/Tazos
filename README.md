# Tazos Kanto

Juego móvil 3D de tazos con los 151 Pokémon originales. Tiras un tazo contra una pila y
te quedas con todos los que consigas dar la vuelta.

- **Documento de diseño completo**: [`docs/DESIGN.md`](docs/DESIGN.md) — concepto, controles
  táctiles, física, efecto palanca, tecnología, arquitectura, roadmap y Fase 0.
- **Prototipo (Fase 0)**: [`unity/`](unity/) — Unity 6, todo por código, sin escenas.
- **Prototipo jugable en el navegador**: [`web/index.html`](web/index.html) — la misma
  física escrita a mano en JavaScript, para poder tirar desde el móvil sin instalar nada.

## Estado

Fase 0: prototipo físico. Mesa, pila de 10 tazos + 6 sueltos, lanzamiento con los cinco
canales de control táctil, física completa de discos, mecánica de palanca, detección de
volteo, combos y contador de Pokédex. Sin tienda, monedas, cuentas ni multijugador: eso
no arregla un lanzamiento que no se sienta bien.

## Cómo probarlo desde el móvil

Abre `web/index.html`. Es un solo fichero sin dependencias: motor de cuerpos rígidos,
render 3D sobre canvas y los cinco canales de control táctil. Sirve para valorar la
sensación del lanzamiento, no para juzgar el acabado.

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
