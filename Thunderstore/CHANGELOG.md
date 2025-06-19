`1.3.0`

- Fixed things just straight up ignoring configs???? 
- Added AutoConfigs and config syncing
- Fixed item descriptions being incorrect

- Fixed Boss attacks being out of order
- Removed timed phases in favor of 3 health-based phases
- Enrage mode P3
- Added adaptive armor to boss
- Shortened laser duration
- Added brief invuln time on spawn
- Improved broken armor debuff to fit better with current stats

- Titanic Sword mostly unchanged
- 5% for 1250% TOTAL damage
- Many bugfixes

- Golden Knurl now grants 33 hp/lvl, 3 regen/level, 3 armor/level
- Should make it a bit different from the normal Knurl
- Still on the chopping block for a full rework, this will do for now.

- Laser Eye now scales with 2 (+2 per stack) enemies hit for 600% base dmg each
- Laser Eye stacks no longer are timed
- Laser Eye stacks start at 25, and is reduced by earning gold. 
- Each time you can afford a small chest, the stacks will hit 0 and the eye will be armed.

`1.2.1`

- Bugfix lasereye NRE

`1.2.0`

- Moved to new maintainer, full backend rewrite.
- Removed Gold Elites
- Modified some cfg options

`1.1.1`
- Recompiled to work with latest game ver

`1.1.0`
- Fixed icons not loading via YUV2 or whatever, if you had issues with this, the mod should load fine now.
- Fixed Laser Eye not working, dnSpy sucks so real

`1.0.4`
- Fixed a cascading NRE spam caused by a really edge case incompatibility between RealerRisingTides and this mod, and maybe between SS2 and this mod.

`1.0.3`
- Added more configs for the balance nerds (proc coeffs, health, damage, elite cost, etc)

`1.0.2`
- Added WorldUnique tag to the 3 items this mod adds. This should stop them from being dropped as teleporter rewards.

`1.0.1`
- Blacklisted the items from showing up in Printers, oops

`1.0.0`
- Now maintained by me
- dnSpied and fixed up for SotS, took a whole day but its worth it

`0.6.5`
- Forgot to include the actual update dll lol

`0.6.4`
- Pretty sure I finally fixed the bug with Golden Knurl's health increase that's been in the mod forever

`0.6.3`
- Okay but this time I actually fixed it

`0.6.2`
- Fixed for the latest update.

`0.6.1`
- Fixed NREs caused by Golden Knurl's implementation.

`0.6.0`
- Updated for Survivors of the Void.
- Now uses RecalculateStatsAPI which should fix any weirdness or incompatibiltiy issues.

`0.5.30`
- I think I actually fixed the Thunderstore formatting this time.

`0.5.3`
- Fixed the names of Gold elites, should now say "Gold <character name>"

`0.5.2`
- Thank you swuff for offering to write lore descriptions for the items in this mod! Finally adding them in 2 months after I got them.
- Fixed an issue where Aurelionite was becoming invincible before its phase transitions.
- Maybe I'll start actually working on this mod again who knows heehoo

`0.5.1`
- Aurelionite will now use its third phase attacks if its first phase change happens when it is at the final third of its health.

`0.5.0`
- Changed the duration of the debuff Aurelionite gets upon its armor being broken to 10 seconds (down from 20).
- Increased Aurelionite's health by 2x.
- Changed Aurelionite's attacks as the phases of its fight progress (see Change List).
- Added a bonus reward for defeating Aurelionite in only 1 or 2 cycles.
- Fixed text color of Golden Knurl's description.
- Reduced Titanic Greatsword's damage from 2000% to 1250%.
- Added config for Aurelionite's fight changes and the items he can drop.

`0.4.2`
- Fixed issues with Golden Knurl's renderer materials (finally).
- Fixed Aurelionite's invincibilty triggering too early.

`0.4.1`
- Fixed the entire game breaking because I forgot to finish my TakeDamage hook.

`0.4.0`
- Reverted the changes to pickup and description text of the last update.
- Only Halcyon Seed contributes to allied Aurelionite's strength, like before, but it is still three times as powerful.
- Reworked Aurelionite's fight (see Change List).

`0.3.3`
- Updated pickup and description text of the items.

`0.3.2`
- All items that Aurelionite can drop now contribute to its allied version's strength. Only Halcyon Seed is removed from the player's inventory, and it now contributes 3 times as much to its strength.
- Updated icons for Golden Knurl and the Gold elite buff.

`0.3.1`
- Updated icon outlines (thanks to swuff for the icons themselves and rob for hooking me up).

`0.3.0`
- All items now have proper icons (thanks to Lamda Male) and item displays.
- Gold Elites now drop 5x the amount of gold that they did before, and have a proper buff icon (thanks to Lambda Male).
- Fixed issues with Coven of Gold's name tokens.
- Fixed issue with Golden Knurl not granting regen.
- Fixed issue with Aurelionite being unable to drop Halcyon Seed.
- Fixed plugin name.

`0.2.2`
- Fixed incompatibility issues with Aetherium.

`0.2.1`
- Golden Knurl now gives a 10% health increase rather than a flat 60.

`0.2.0`
- Gave the Gold Elites their unused skin.
- Updated Titanic Greatsword's model.
- Added Guardian's Eye.

`0.1.1`
- Fixed for RoR2 v1.1.1.2.

`0.1.0`
- Initial Unfinished Release