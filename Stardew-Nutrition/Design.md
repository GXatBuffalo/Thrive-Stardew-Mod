# Stardew Sustainable Farming – Design Document

**Author:** SomebodyUnown 
**Date:** 2025-08-12
**Status:** In Progress

---

## 1. Overview

Project Overview
This is a Stardew Valley mod that aims to increase complexity in the base farming mechanics, encouraging players to think beyond raw profitibility of a crop. Instead of focusing solely on profit per day, players will need to consider soil health, crop diversity, and long-term sustainability in their farming strategies.

Motivation
Whether in vanilla Stardew or a modded Stardew with tons of popular crop mods, there’s often little reason to plant anything other than the most profitable crops. This mod changes that by introducing mechanics that reward diversity and careful resource management, making less-utilized crops, flowers, and other plants (and fungi) more viable.

Design Philosophy
While not a 1:1 simulation of real-world agriculture, the mod does draw inspiration from real-life parallels. In reality, you cannot endlessly extract resources from the same soil without consequences, and every crop has an environmental impact. This mod aims to mirror that idea in a fun and approachable way, prompting players to think about sustainability while still enjoying the game.

---

## 2. Key Features
- Track soil nutrients (tentative names: nitro, phos, pH, iridium, mana)
- Adjust crop quality and nutritional value based on soil quality during growth
- Each crop has randomized soil depletion (or replenishment) rates that are persistent per save. New or different saves will have new values that encourage the player to discover an entirely new array of crops to manage.
- Compatibility with mods such as Wildflour's Atlier Goods.
- Social events with Stardew characters (both vanilla and modded) to integrate mechanics and lore.

---

## 3. Architecture
### 3.1 Main Components
### 3.2 Data Flow


---

## 4. Game/Tech Integration
- **Framework:** SMAPI (Stardew Valley Modding API), ContentPatcher
- **Language:** C#

---

## 5. Planned Roadmap
### v0.1 – Basic Core Systems
- [x] Load or create crop and soil data
- [x] Daily nutrient depletion and corresponding bonuses/penalties
- [ ] Adjust crop yields based on its overall health during growth.

### v0.2 – Balancing
- [ ] Write script to analyze mechanic formulas and their results
- [ ] Adjust nutrient depletion formulas 

### v0.3 - Game Integration
- [ ] Hook mechanics into game start, save start, day start, harvest, planting, and tool use, etc.
- [ ] Add SMAPI console logging to check for bugs and game state changes
- [ ] Custom tool/item to measure soil and crop health.
- [ ] Custom items to improve soil quality
- [ ] Discourage farming in non-farm maps both to improve mod performance and for lore purposes

### v0.4 - Basic Compatibility and Edge Cases
- [ ] Add compatibility with crops labled as magic
- [ ] Check for and add logic to include "crop-like", growable items that are coded differently.
- [ ] Adjust depletion and bonuses based on 'mana'
- [ ] (Possible) Introduce additional mechanics based on context tags of a crop or seed.

### v0.5 - Player Interactions and User Interfaces
- [ ] HUD elements for viewing soil health for custom tool/item
- [ ] HUD elements for viewing crop depletion or replenishment rates for custom tool/item
- [ ] Menu to look at all known data
- [ ] Menu to look at rough known data of current map.
- [ ] Integration with mod 'Better Game Menu'

### v0.6 - Code Quality & Configurability
- [ ] Update code allowing for adjustment of soil variables.
- [ ] Configuration file to adjust mechanics
- [ ] i18n support.
- [ ] Refactor code for readibility and add comments to explain code.

### v0.7 - Beta Test
- [ ] Recruit testers from modding community to gather performance metrics and bug reports.
- [ ] Collect professional feedback from experienced mod authors and players.
- [ ] Ask for code review from experienced mod authors.
- [ ] Future-proof code based on feeback gathered.

### v??? - Lore Integration
- [ ] Mail from Demetrius informing the user of basic mechanics
- [ ] Event with Demetrius to encourage consideration of mechanics and to give player ability to analyze crop/soil stats.
- [ ] Event with various other farmers (???)
- [ ] ConversationTopics depending on how well player manages their farm and its crops/soil.

### v0.9 - Beta Test 2
- [ ] Collect narrative feedback and bug reports
- [ ] Fix issues and improve event writing for pacing, tone, and/or clarity

### v1.0 - Release
- [ ] Add GitHub release with changelog and installation instructions
- [ ] Write a mod description that compels players but also effectively informs and publish on nexusmods.
- [ ] Gather player feedback for potential post-launch patches

### v2.0 - Nutrition Mechanics for Characters

### v3.0 - Expanded Sustainable Farming
- [ ] Deeper mechanics involving pests or symbiotic animals or other organisms

### v3.2 - Lore Integration with Newly Relevant Characters


---

## 6. Trade-offs & Decisions


---

## 7. References
- SMAPI Docs: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/SMAPI
- Stardew Valley decompiled code (via ILSpy)
- Original game mechanics notes
