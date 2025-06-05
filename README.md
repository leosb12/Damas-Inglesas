# Damas Inglesas - Windows Forms

Este proyecto implementa el clásico juego de Damas Inglesas utilizando **C# con Windows Forms**. Incluye una interfaz gráfica interactiva, lógica de movimiento, captura obligatoria, cronómetro, sonidos personalizados y más.

---

## 🎮 Características del Juego

- Turnos alternos entre jugadores (rojo y azul).
- Movimiento y coronación (reinas).
- **Captura obligatoria** si hay fichas rivales disponibles.
- Reproducción de sonidos personalizados (`.wav`).
- Cronómetro en tiempo real.
- Contador de movimientos por color.
- Registro en pantalla de cada movimiento (estilo notación).

---

## 🧠 Estructuras y Componentes Usados

### 📦 Controles de interfaz (UI)

- `PictureBox[] rojas` y `PictureBox[] azules`: arreglos para manejar dinámicamente las fichas rojas y azules.
- `TextBox`: para mostrar los movimientos realizados.
- `Label`: para mostrar el turno, cronómetro y contadores de movimiento.

### 🧮 Lógica del juego

- **Vectores/Arreglos**:
  - Se utilizan dos vectores (`PictureBox[]`) de longitud 13 (posición 0 no se usa) para representar las fichas en juego.
  - Esto facilita el recorrido, validación de posiciones, coronación y eliminación de fichas.

- **Matriz implícita**:
  - El tablero no se modela con una matriz explícita, sino que las posiciones `(X, Y)` en `Point` representan las coordenadas de cada cuadro (50x50 px cada uno), simulando así una cuadrícula de 8x8.
  
- **Timers**:
  - `Timer` de Windows Forms junto con `Stopwatch` para implementar el cronómetro.

- **Lógica de coronación**:
  - Se asigna una propiedad `Tag = "reina"` a las fichas que llegan al otro extremo del tablero.

- **Validación de captura obligatoria**:
  - Antes de permitir un movimiento, se evalúa si existen fichas enemigas disponibles para ser capturadas. En caso afirmativo, se impide mover si no se realiza la captura.

---



