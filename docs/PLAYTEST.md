# Primera prueba de Unity

Esta prueba valida la sensación del lanzamiento, no el acabado visual ni una build móvil.

## Preparación en Windows

1. Instala **Unity Hub** y el editor **Unity 6.0.32f1** con la plantilla **Universal 3D**.
2. Descarga o clona este repositorio y crea un proyecto nuevo desde Unity Hub: **Universal 3D**.
3. Copia la carpeta `unity/Assets/Scripts/` de este repositorio dentro de `Assets/Scripts/` del proyecto nuevo.
4. En Unity abre **Edit → Project Settings → Player** y ajusta **Active Input Handling** a **Both** o **Input Manager (Old)**.
5. Abre una escena vacía y pulsa **Play**. `Bootstrap` crea automáticamente la mesa, cámara, luces, HUD y partida; no hay escena que preparar.

## Cómo jugar en ordenador

- Arrastra el ratón hacia atrás desde cualquier zona vacía de la mesa y suéltalo.
- Un arrastre largo produce más potencia.
- La dirección del lanzamiento es la contraria al arrastre.
- Antes de soltar, un movimiento rápido hacia arriba da más elevación; lateral da inclinación; dibujar una curva aporta spin.
- Usa **Cambiar tazo** para alternar entre Snorlax, Geodude, Scyther, Pikachu y Mewtwo.
- Abre **Física** para ajustar *Palanca* y *Vuelco en canto*. Empieza en 2,2 y 2,5 respectivamente.

## Qué observar durante 10 partidas

1. ¿Se entiende el gesto sin explicarlo?
2. ¿Algún tiro produce al menos un vuelco? Objetivo inicial: 70–80 % de los tiros.
3. ¿Una partida consigue entre 4 y 8 tazos de 16?
4. ¿El jugador quiere repetir el tiro tras fallar?

Si el resultado es errático, toca primero los dos ajustes de física; no añadas contenido todavía. Anota el tazo usado, los valores de Palanca/Vuelco y el resultado de la partida para poder repetir los hallazgos.
