# TAZOS KANTO — Documento de diseño (v1)

Juego móvil 3D de tazos con los 151 Pokémon originales.
Objetivo de sensación: **80% física creíble + 20% arcade**.
Frase que debe provocar el juego: *"voy a tirar una vez más a ver cuántos volteo"*.

---

## 1. Concepto final

Una mesa. Una pila de 15–20 tazos boca abajo. Cinco lanzamientos.
Todo lo que quede **boca arriba** cuando la mesa se detenga es tuyo y entra en la Pokédex.

No hay combate, ni turnos, ni estadísticas RPG. El Pokémon **es** el tazo: define masa,
grosor, canto, fricción y rebote. Elegir a Snorlax o a Scyther es elegir una herramienta.

El bucle de una partida (2–5 min):

1. **Reparto**: 15–20 tazos aleatorios de Kanto, boca abajo, en pila + satélites.
2. **Tiro 1–2 (demolición)**: buscas el impacto que revienta la pila. Vuelcos en cadena.
3. **Tiro 3–5 (rebusca)**: quedan tazos sueltos, algunos con un borde levantado.
   Aquí se juega fino: golpe preciso al canto, o **palanca** por debajo.
4. **Recuento**: se muestran los Pokémon conseguidos, combos y % de Pokédex.

Nada de la partida se decide por probabilidad. Si un tazo se da la vuelta es porque
la simulación lo volteó. Lo "arcade" (ese 20%) está en tres sitios muy acotados y
declarados: amplificación del par en contactos de cuña (§4), asistencia sutil de
puntería en el primer tiro y presentación (cámara lenta, háptica, sonido).

**Tono visual**: mesa de patio/parquet noventero, luz cálida, tazos con brillo plástico,
cámara baja y cercana. Nostalgia de 1996 con render moderno.

---

## 2. Diseño de los controles táctiles

Un solo pulgar, cinco canales de control. El tirador es siempre el mismo gesto: **tirar
hacia atrás y soltar** (tirachinas), pero *cómo* tiras hacia atrás y *cómo* sueltas
cambia el vuelo.

| Canal | Gesto | Rango |
|---|---|---|
| **Dirección** | Ángulo del vector de arrastre (se lanza en sentido opuesto) | 360°, útil ±60° |
| **Potencia** | Longitud del arrastre (0 → 35% del alto de pantalla) | 4 – 14 m/s |
| **Ángulo de salida** | Componente vertical del *flick* al soltar (velocidad del dedo en los últimos 80 ms) | 0° (rasante) – 35° (globo) |
| **Inclinación (roll)** | Componente lateral de ese mismo flick | −45° a +45° respecto al plano de la mesa |
| **Spin** | Curvatura acumulada del recorrido del dedo (rotación del vector de arrastre durante el arrastre) | −25 a +25 rad/s |

Por qué así: dirección y potencia son continuas y las controla el ojo; el ángulo, la
inclinación y el spin salen del **gesto de soltar**, que es exactamente lo que pasa al
lanzar un tazo de verdad con la muñeca. Se aprende sin tutorial y tiene techo alto.

HUD durante el apuntado: una flecha de potencia, un disco fantasma que muestra
**inclinación real** del tazo y un arco de spin. Trayectoria prevista sólo en los dos
primeros tiros de la primera partida (se retira después: molesta y quita tensión).

Dos tiros arquetípicos que el sistema debe permitir:

- **El demoledor**: arrastre largo y recto, flick fuerte hacia arriba, poco spin.
  Llega alto, cae de canto sobre la pila.
- **El rasante de palanca**: arrastre corto, flick horizontal, inclinación ~15°, spin alto.
  El tazo patina pegado a la mesa y **se mete debajo** del borde del objetivo.

---

## 3. Funcionamiento de la física

Todo son rigid bodies. Ninguna regla de "este tazo se voltea".

**Forma**: cada tazo es un cilindro de radio 0,04 m con **canto biselado** y una
**curvatura muy leve** (domo de ~0,6 mm). Eso es clave: un tazo real apoyado tiene el
borde ligeramente despegado de la mesa, y ese hueco de menos de un milímetro es lo que
permite que otro tazo se cuele por debajo. Sin domo no hay palanca.
Collider: MeshCollider **convexo** generado con la misma silueta (no un cilindro
primitivo, que redondea el canto y mata la cuña).

**Parámetros por Pokémon** (arquetipo + ajustes a mano):

| Arquetipo | Masa | Grosor | Fricción | Rebote | Papel |
|---|---|---|---|---|---|
| Pluma (Gastly, Zubat) | 0,55× | fino | baja | alto | patina y salta, impredecible |
| Ligero (Pikachu) | 0,75× | medio | media | medio | preciso, poco daño |
| Equilibrado | 1,0× | medio | media | medio | todoterreno |
| Denso (Blastoise) | 1,3× | grueso | media-alta | bajo | empuja sin descontrolarse |
| Pesado (Snorlax, Tauros) | 1,8× | muy grueso | alta | muy bajo | revienta pilas |
| Roca (Geodude, Onix) | 1,6× | grueso | muy alta | muy bajo | no rebota, arrasa |
| Filo (Scyther, Kabutops) | 0,8× | muy fino, bisel agresivo | baja | bajo | rey de la palanca |
| Precisión (Mewtwo, Alakazam) | 1,0× | medio | media | bajo | spin estable, trayectoria limpia |

**Simulación**: `fixedDeltaTime = 1/120`, solver iterations 12 (24 en velocidad),
`Physics.defaultSolverVelocityIterations` alto, colisión continua en el tazo lanzado
(a 14 m/s con 4 mm de grosor, discreto lo atraviesa todo). Sleep threshold bajo para
que los tazos casi parados no se duerman antes de decidir su cara.

**Vuelo**: gravedad normal + arrastre aerodinámico dependiente del área proyectada
(un tazo de canto vuela; de plano, frena). El spin sobre el eje del disco da
estabilidad giroscópica: mucho spin = trayectoria fiable; poco spin = el tazo cabecea
y llega de cualquier manera. El spin lateral genera una fuerza tipo Magnus suave que
curva el tiro. Al tocar la mesa, el spin se convierte en fricción rotacional: un tazo
con spin **derrapa y sigue caminando** entre la pila en vez de pararse.

**Impacto**: nada especial. Restitución y fricción por material, transferencia de
momento normal. Los vuelcos en cadena salen solos porque los tazos apilados están
apoyados unos en otros y su centro de masa está muy cerca del borde de apoyo.

**Detección de volteo**: un tazo cuenta como conseguido cuando (a) está en reposo
(velocidad lineal < 0,05 m/s y angular < 0,4 rad/s durante 0,35 s), (b) está apoyado
en la mesa, y (c) `dot(up_tazo, up_mundo) > 0,55`. Se comprueba **al final del turno**,
no durante, para que la cadena de rebotes pueda deshacer un volteo — eso es parte de
la tensión.

---

## 4. Funcionamiento del efecto palanca

No es un minijuego ni un botón. Es una consecuencia de la geometría.

Secuencia física real que el motor reproduce:

1. El tazo lanzado llega **rasante e inclinado**; su borde de ataque va más bajo que
   el borde levantado del objetivo (ese hueco del domo, o un borde ya alzado por un
   impacto anterior).
2. Entra en la cuña. Los contactos que se generan están **por debajo del plano medio**
   del objetivo y hacia un lado de su centro.
3. Esos contactos producen un par sobre el eje que forma el borde opuesto, que hace de
   fulcro. Si el momento supera el peso del tazo por su brazo, vuelca.

El único añadido arcade: cuando el detector ve un **contacto de cuña legítimo**
(punto de contacto bajo el plano medio del objetivo, dentro de su radio, con velocidad
relativa entrante), multiplica ese par por un factor ajustable (arranca en 2,2). No
inventa fuerzas ni decide resultados: **amplifica un par que ya existe**, porque a
escala de 4 mm y 60 Hz la simulación pierde parte del empuje real de la cuña.
Factor 1,0 = física pura y sigue funcionando, sólo que exige más precisión.

Consecuencias de diseño que esto regala gratis:

- Un tazo **medio levantado** apoyado en otro es un objetivo mucho más fácil después:
  su hueco es de centímetros, no de milímetros. El jugador aprende a *dejar* tazos así.
- Los tazos **Filo** (Scyther, Kabutops, Aerodactyl) entran donde otros no caben.
- Un **Snorlax** rasante no hace palanca: es demasiado grueso para colarse. Cada tazo
  sirve para una fase distinta de la partida. Ahí está la decisión interesante.

---

## 5. Tecnología recomendada

**Unity 6 (URP, Mobile Renderer) + C#.** Es la elección correcta aquí, y no por inercia:

- **Física**: PhysX con substepping y colisión continua, que es justo lo que exige un
  disco fino a 14 m/s. El motor de Godot (Jolt en 4.x) ya es bueno, pero PhysX tiene
  más años de rodaje con contactos rasantes y colliders convexos finos, que es
  exactamente nuestro caso límite.
- **iPhone desde Windows**: se desarrolla y se itera al 100% en Windows; Unity genera
  un proyecto Xcode. Sólo hace falta un Mac (o un servicio de build en la nube tipo
  Unity Build Automation / Codemagic) para firmar y subir a TestFlight. Ese cuello de
  botella es idéntico en Godot y en cualquier motor: es requisito de Apple, no del motor.
- **Rendimiento**: 20 discos, un material, GPU instancing → 60 FPS sobrados en un
  iPhone moderno; el coste real está en la física, no en el render.
- **Ecosistema**: háptica nativa iOS, Cinemachine, DOTween, y salida a Android
  sin tocar el código.

Godot 4 sería mi segunda opción (más ligero, abierto, exporta a iOS también desde
Windows) y descartaría Unreal (peso y complejidad excesivos para 20 discos).

Versión: **Unity 6.0 LTS**, URP, Input System nuevo, `IL2CPP + ARM64`.

---

## 6. Arquitectura mínima

```
Bootstrap ──► GameSession ──► ThrowController ──► Tazo (rigidbody)
                  │                                  │
                  ├─► TazoFactory ◄── PokedexData ────┤   (151 especies → perfil físico)
                  ├─► FlipJudge  ◄────────────────────┘   (reposo + orientación)
                  ├─► Feel        (cámara lenta, háptica, sonido)
                  ├─► CameraRig
                  ├─► Hud
                  └─► Collection  (persistencia Pokédex, JSON local)
```

Reglas: la física no conoce la UI; `FlipJudge` sólo lee estado, nunca lo fuerza;
`PokedexData` es datos puros (151 entradas + arquetipo), sin lógica. Todo el prototipo
se construye por código al arrancar (`Bootstrap`), sin escena que mantener, para que
iterar sea editar un número y darle a Play.

---

## 7. Roadmap por fases

| Fase | Contenido | Criterio de salida |
|---|---|---|
| **0. Prototipo físico** *(esta entrega)* | Mesa, 10–20 tazos, lanzamiento, colisiones, spin, detección de volteo, palanca, contador | Tirar es divertido sin nada más |
| **1. Feel** | Cámara lenta en el impacto, háptica, sonido de plástico, combos, cámara dinámica | Apetece repetir el tiro aunque falles |
| **2. Partida** | 5 tiros, reparto por niveles, recuento final, elección de tazo lanzador | Sesión de 3 min completa y con tensión |
| **3. Colección** | Pokédex 151, pantalla de álbum, sprites/modelos de los tazos | Se ve el progreso y engancha |
| **4. Contenido** | Layouts de pila variados, retos, versiones holo/dorado/metálico | Razón para volver mañana |
| **5. Plataforma** | Pulido iOS, TestFlight, Android | Build firmada y jugable por terceros |

Fuera del alcance por ahora, y de forma deliberada: tienda, monedas, cuentas,
multijugador. Nada de eso arregla un lanzamiento que no se sienta bien.

---

## 8. Diseño de la primera versión jugable (Fase 0)

Lo que se entrega en este repositorio:

- Mesa 3D con bordes, cámara en 3/4 baja adaptada a vertical (FOV corregido por aspecto).
- Pila de 10 tazos boca abajo + 6 satélites sueltos alrededor.
- Tazo lanzador con Pokémon elegible (Snorlax / Scyther / Pikachu / Mewtwo / Geodude
  por defecto, los 151 disponibles en datos).
- Los 5 canales de control táctil de §2, con HUD de potencia, inclinación y spin.
- Física completa de §3: domo, bisel, perfiles por arquetipo, arrastre aerodinámico,
  Magnus, colisión continua, 120 Hz.
- Palanca de §4 con factor de amplificación ajustable en vivo.
- Detección de volteo al terminar el turno + combos (Double / Triple / Quad Flip,
  Avalanche a partir de 5) + cámara lenta y háptica.
- Contador de Pokémon conseguidos y recuento al agotar los 5 tiros.

**Qué hay que medir en esta fase** (y ajustar antes de seguir): porcentaje de tiros que
voltean al menos uno (objetivo 70–80%), tazos volteados por partida (objetivo 4–8 de 16),
y cuántas partidas seguidas juega alguien sin que se lo pidas. Si ese último número es
bajo, se arregla la física, no se añade contenido.
