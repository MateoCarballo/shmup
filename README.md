# 🚀 SHMUP - Shoot 'Em Up en Unity

![Unity](https://img.shields.io/badge/Unity-2021.3+-black?logo=unity&logoColor=white)
![License](https://img.shields.io/github/license/MateoCarballo/shmup)
![Languages](https://img.shields.io/github/languages/top/MateoCarballo/shmup)



> Un clásico moderno de los shoot 'em up, desarrollado en Unity. ¡Sobrevive a oleadas de OVNIs enemigos, esquiva disparos, recoge power-ups y salva la galaxia!

---
# 🚀 Velocidad Total - Enalce a itch.io 
<p align="center">
  <img src="https://img.shields.io/badge/ITCH.IO-FA5C5C?style=for-the-badge&logo=itch.io&logoColor=white" alt="itch.io">
  <a href="https://srcalico.itch.io/velocidad-total">
    <img src="https://img.shields.io/badge/JUEGA_AHORA-FF5722?style=for-the-badge&logo=gamejolt&logoColor=white" alt="Juega ahora">
  </a>
</p>

## 🎮 ¿Qué es un Shoot 'Em Up?

Los **Shoot 'Em Up** (SHMUP) son un subgénero de los videojuegos de acción donde el jugador debe enfrentarse a múltiples enemigos, normalmente voladores, disparando sin cesar mientras evita sus ataques. Este género nació en la década de 1980 con títulos icónicos como:

- **Space Invaders (1978)**
- **Galaga (1981)**
- **R-Type (1987)**
- **Gradius (1985)**

Este proyecto rinde homenaje a estos clásicos, integrando mecánicas modernas con una estética retro.

---

## 🎮 Mecánicas del Juego

- Controlas una **nave espacial**.
- Tu objetivo es **sobrevivir a oleadas de enemigos (OVNIs)** en **2 niveles distintos**.
- Los enemigos se mueven con **patrones predefinidos** y **disparan proyectiles**.
- El jugador debe:
  - **Esquivar los disparos enemigos**.
  - **Contraatacar** con su propio disparo frontal.
  - **Eliminar enemigos** antes de que abandonen la pantalla (saliendo por los triggers de autodestrucción).

---

## 🎁 Power-ups

Al eliminar enemigos, estos pueden **soltar aleatoriamente (100% en pruebas)** uno de los **3 power-ups** disponibles:

| Power-up               | Efecto                                      | Duración     |
|------------------------|---------------------------------------------|--------------|
| 🛡️ Escudo             | Absorbe un disparo o desaparece con el tiempo | Limitada     |
| ⚡ Velocidad extra     | Aumenta la velocidad de movimiento           | Limitada     |
| 🔫 Disparo en cono     | Dispara múltiples proyectiles en abanico     | Limitada     |

> **Nota:** Los power-ups tienen **duración fija** (simplificación del código). El escudo se desactiva al recibir daño o al agotarse el tiempo.

---

## 💥 Enemigos

- Se desplazan con **patrones de movimiento definidos**.
- Si no son destruidos en un cierto tiempo, **salen de la pantalla** y se autodestruyen mediante **triggers fuera del área visible**.
- La transición al siguiente nivel ocurre:
  - Al eliminar al último enemigo del nivel.
  - O tras un tiempo límite desde que el último enemigo aparece.

---

## ❤️ Sistema de Vidas

- Comienzas con **3 vidas + la activa (total 4)**.
- Cada impacto sin escudo resta una vida.
- El juego termina al perder todas las vidas.

---

## 🧪 Estado del Proyecto

Este proyecto se encuentra en desarrollo y se utiliza para **pruebas de mecánicas básicas**. Por ejemplo:

- Power-ups siempre aparecen (probabilidad del 100%) para facilitar el testeo.
- Duraciones de enemigos y power-ups son fijas.

---

## 🎨 Assets Utilizados

Este juego utiliza el pack de assets gratuitos de [Kenney - Space Shooter Redux](https://kenney.nl/assets/space-shooter-redux), ubicados dentro del proyecto en:

```
Assets/Kenney/space-shooter-redux/
```

---

## 🛠️ Tecnologías

- **Unity** 2021.3+
- **C#**
- **Visual Studio / VSCode**
- **Sistema InputManager**
- **Sprites 2D y Partículas**

---

## 📁 Estructura Recomendada (Ejemplo)

```
Assets/
├── Audio/
│   └── SFX/
├── Prefabs/
├── Scenes/
├── Scripts/
├── Sprites/
│   └── BackGround/
│   ├── PNG/
│       └── **Contiene assets no usados de kenney/

```

---

## 🚀 Cómo Ejecutar

1. Clona el repositorio:
   ```bash
   git clone https://github.com/MateoCarballo/shmup.git
   ```

2. Abre el proyecto en Unity (2021.3 o superior).

3. Abre la escena principal (`Assets/Scenes/MainMenu.unity`).

4. Pulsa ▶️ "Play" para iniciar el juego.

---

## 🔮 Posibles Mejoras Futuras

- Duraciones y probabilidades dinámicas (aleatoriedad).
- Nuevas oleadas y niveles.
- Mayor variedad de enemigos.
- Sistema de puntuación y ranking.

---

## ✉️ Contacto

Desarrollado por [Mateo Carballo](https://github.com/MateoCarballo).

¡Siente libertad de contribuir, dar ⭐ y compartir el proyecto!

---

