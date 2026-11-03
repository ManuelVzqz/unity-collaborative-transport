# Multi-Agent Reinforcement Learning for Collaborative Object Manipulation in Unity-Based Industrial Simulation Environments

[![Paper](https://img.shields.io/badge/Paper-IEEE_Latin_America_Transactions-blue.svg)](#) **Submission ID:** 10973 *(Conditionally Accepted)*

This repository contains the Unity ML-Agents environment, source code, hyperparameter configurations, and standalone executables for our research on collaborative object manipulation using Multi-Agent Reinforcement Learning (MARL) (Manuscript ID: 10973).

## Authors and Affiliations

1. **Manuel E. Vázquez** (`manuel.vazquez@fio.unicen.edu.ar`) — *Facultad de Ingeniería, Universidad Nacional del Centro de la Provincia de Buenos Aires (UNCPBA), and Centro de Investigaciones en Física e Ingeniería del Centro (CIFICEN), UNICEN-CICpBA-CONICET, Olavarría (B7400JWI), Buenos Aires, Argentina.*
2. **Carolina Saavedra Sueldo** (`carolina.saavedra@fio.unicen.edu.ar`) — *Facultad de Ingeniería, UNCPBA, and CIFICEN (UNICEN-CICpBA-CONICET), Olavarría (B7400JWI), Buenos Aires, Argentina.*
3. **Ivo Perez Colo** (`ivo.perez@fio.unicen.edu.ar`) — *Facultad de Ingeniería, UNCPBA, and CIFICEN (UNICEN-CICpBA-CONICET), Olavarría (B7400JWI), Buenos Aires, Argentina.*
4. **Luis O. Ávila** (`loavila@unsl.edu.ar`) — *Laboratorio de Sistemas Inteligentes, Universidad Nacional de San Luis (UNSL), San Luis, Argentina, and CONICET, Argentina.*
5. **Sebastián A. Villar** (`svillar@fio.unicen.edu.ar`) — *Facultad de Ingeniería, UNCPBA, and CIFICEN (UNICEN-CICpBA-CONICET), Olavarría (B7400JWI), Buenos Aires, Argentina.*
6. **Agustina Jurgiel** (`ajurgi@palermo.edu`) — *Facultad de Ingeniería, Universidad de Palermo, Mario Bravo 1050, C1175 ABT, Ciudad Autónoma de Buenos Aires, Argentina.*
7. **Gerardo G. Acosta** (`ggacosta@fio.unicen.edu.ar`) — *Facultad de Ingeniería, UNCPBA, and CIFICEN (UNICEN-CICpBA-CONICET), Olavarría (B7400JWI), Buenos Aires, Argentina.*
8. **Mariano De Paula** (`mariano.depaula@fio.unicen.edu.ar`) — *Facultad de Ingeniería, Universidad de Palermo, C1175 ABT, Ciudad Autónoma de Buenos Aires, Argentina, and CIFICEN (UNICEN-CICpBA-CONICET), Olavarría (B7400JWI), Buenos Aires, Argentina.*

---

<p align="center">
  <img src="images/Escenario.png" alt="Environment Overview" width="700">
</p>

## Overview
In modern flexible manufacturing systems, autonomous robots must learn collaborative behaviors without constant reprogramming. This project simulates a cooperative task where two agents must transport a shared object to a designated delivery zone. We evaluate three decentralized algorithms: **PPO, SAC, and MA-POCA**.

The project evaluates the agents in two distinct scenarios:

### 1. Base Environment (Obstacle-Free)
Designed to evaluate pure coordination and basic collaborative transport.
<p align="center">
  <img src="images/escenario_base.png" alt="Base Environment" width="400">
</p>
<p align="center">
  <em>Learned trajectory sequence:</em><br>
  <img src="images/TB_1.png" width="250"> <img src="images/TB_2.png" width="250"> <img src="images/TB_3.png" width="250">
</p>

### 2. Complex Environment (With Obstacles)
Transportation requires spatial awareness and synchronized maneuvering to avoid the central obstacle before reaching the target.
<p align="center">
  <img src="images/escenario_complejo.png" alt="Complex Environment" width="400">
</p>
<p align="center">
  <em>Learned trajectory sequence avoiding the obstacle:</em><br>
  <img src="images/TC_1.png" width="250"> <img src="images/TC_2.png" width="250"> <img src="images/TC_3.png" width="250">
</p>

---

## Repository Structure
* `Assets/`: Unity source code, C# agent scripts, 3D models, and scene files.
* `Packages/`: ML-Agents dependencies (`manifest.json`).
* `Config_yaml/`: Training hyperparameter configurations (`configPOCA.yaml`, `configPPO.yaml`, `configSAC.yaml`).
* `Resources/`: Contains the pre-compiled Windows standalone builds compressed as `.zip` files (`Ejecutable_simple.zip` and `Ejecutable_complejo.zip`).
* `images/`: Visual assets showing environment layouts and learned trajectories.

## 🎮 Running the Pre-compiled Environments
You do not need to install Unity to test the behaviors. Pre-compiled Windows standalone builds are available in the **Releases** section of this repository.

1. Navigate to the **Releases** section on the right side of this GitHub page.
2. Download the `Ejecutable_simple.zip` or `Ejecutable_complejo.zip` archives.
3. Extract the folder and run the `.exe` file inside to observe the agents performing the task.

## Running from Source (Unity)
To open the project and train the models yourself, ensure you are using a compatible Unity version (e.g., Unity 2022.3) and the corresponding ML-Agents Release package. 

### Training Configurations
The `Config_yaml` folder contains the exact hyperparameters used to train the agents in our research. You can start training by running the ML-Agents CLI with the desired config file. For example:

`mlagents-learn Config_yaml/configSAC.yaml --run-id=SAC_Run1`

### Agent Scripts and Architecture
Due to the architectural differences in how ML-Agents handles individual versus group rewards, we provide two separate agent scripts to reproduce the experiments:

1. **For PPO and SAC (Independent architectures):** Assign the `AgenteColaborador_PPO_SAC.cs` script to the agents. This utilizes individual credit assignment (`AddReward`).
2. **For MA-POCA (Centralized architecture):** Assign the `AgenteColaborador.cs` script. This requires the `SimpleMultiAgentGroup` component and utilizes group credit assignment (`AddGroupReward`).
