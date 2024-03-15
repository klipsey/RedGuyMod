# Ravager
- Adds the Ravager, an original new character
- Fully functional base kit with an unlockable mastery skin
- Fully multiplayer compatible!
- Has item displays
- Full Risk of Options support for all configuration options

[![](https://raw.githubusercontent.com/ArcPh1r3/RedGuyMod/main/Release/FuckShit/ss1.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/RedGuyMod/main/Release/FuckShit/ss2.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/RedGuyMod/main/Release/FuckShit/ss3.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/RedGuyMod/main/RedGuyUnityProject/Assets/Ravager/Icons/texRavagerIcon.png)]()

To share feedback, report bugs, or offer suggestions feel free to create an issue on the GitHub repo or join the Discord: https://discord.gg/HV68ujvkqe

___

# Unlock

Currently comes unlocked by default

___

# Skills

## Passive
The Ravager can jump off walls and charge these jumps to perform great leaps.
Also, melee hits fill up the Blood Well, healing him and empowering his skills for a short time when filled.

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/passive.gif?raw=true)]()

## Primary
Basic melee slash

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/primary.gif?raw=true)]()

Empowered: Swing much faster

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/primary2.gif?raw=true)]()

## Secondary
Lunging AoE slash that stuns

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/secondary.gif?raw=true)]()

Empowered: Lunge farther and deal more damage to low health enemies

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/secondary2.gif?raw=true)]()

## Utility
Expunge Blood Well to heal based on the blood consumed

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/utility.gif?raw=true)]()

Empowered: Unleash a devastating blood explosion, along with healing more

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/utility2.gif?raw=true)]()

## Special
Grab an enemy and slam them into the ground, consuming them and healing if the grab kills

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/special.gif?raw=true)]()

Empowered: Drag them along the ground after landing, dealing heavy damage over time and hitting enemies you pass through

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/special2.gif?raw=true)]()

If hitting a boss, instead punch them, firing a shockwave hitting enemies in a cone behind them

[![](https://github.com/ArcPh1r3/RedGuyMod/blob/main/Release/FuckShit/special3.gif?raw=true)]()

___

## Donations
If you enjoy my work and would like to support me, you can donate to my Ko-fi: https://ko-fi.com/robdev

## Credits
rob - Everything

nayDPz - Grab code

Swuff - Feedback, character icon outline

Moffein, TheTimeSweeper - Loads of valuable gameplay feedback

Redacted - Artistic feedback

## Known Issues
- Killing Kjaro and Runald elder lems with Brutalize prevents them from dropping their respective bands
- Goat Hoof and Royal Capacitor item displays delete limbs

## Future Plans
- Proper skins
- More alternate skills
- Ancient Scepter support
- General polish

___

## Changelog

`1.3.15`
- Brutalize's boss cling is now blacklisted on some bosses
- Clinging onto a boss with Brutalize now lets you use your selected utility skill instead of a generic block
- Clinging onto a boss with Brutalize now has a duration of 8 seconds- added config to make this infinite again
- Fixed unintended animation cancel on Twisted Mutation
- Fixed weird inconsistencies when grabbing bosses with empowered Brutalize
- Fixed Brutalize sometimes teleporting you when grabbing enemies

`1.3.14`
- Twisted Mutation reworked- now a charging blink similar to default passive, still requires no terrain, still refreshes on hit
- Legacy version is available via cursed config, also lowered health cost from 10% to 5%
- Holding secondary while releasing Nullify now makes it do an AoE blast instead
- Added animations to indicate this and the primary mechanic
- Grounded empowered Cleave no longer deals bonus damage to low health targets- instead, each enemy hit refreshes the lunge and boosts the damage to the next target by 50% (stacking)
- Pummel is now non-lethal
- The shockwave following Pummel is now guaranteed to hit its primary target
- Brutalize no longer punches bosses, now clings onto them allowing you to slash them and recast for a powerful flourish
- ^m1 slashes for default m1 damage, m2 slower stabs for 600%, util defends, r slashes and jumps off for 1000%
- Readded ground impact, fixed inconsistency and made it much harder to trigger
- Lowered enemy step detection radius, but now can jump off any hurtbox
- ^previously only counted bullseyes, meaning bosses were incredibly inconsistent, this is fixed
- Moved Ravager directly behind Void Fiend in the CSS
- Fixed certain alt skins using the void skin VFX set instead of the default

`1.3.13`
- Removed ground impact until further notice
- Changed mastery skin again, added an alternate due to request
- Updated Voidborn skin- still looks awful but maybe a little bit less so?
- Releasing Nullify above half charge now uses the quick blast instead of the beam if you're holding M1- thanks to tsuyoikenko for this
- Fixed the stupid vanilla lobby skin bug

`1.3.12`
- Reduced frequency of ground impacts and made skills cancel it properly

`1.3.11`
- forgot to remove a line of debugging code

`1.3.10`
- Cleave now forces a sprint (making it agile was screwing with my muscle memory)
- Fixed animation bug

`1.3.9`
- i swear it works this time

`1.3.8`
- 1.3.7 was not playtested

`1.3.7`
- The newly added landing impact can now be cancelled into a charged jump

`1.3.6`
- Added fall damage immunity, replaced it with an impact animation
- Physical Prowess no longer cancels itself when used on the ground
- Increased range of Physical Prowess walljump check, moved closer to the ground to make ground jumps easier
- Cleave no longer cancels sprint
- Nullify charge gained per projectile severely lowered
- Eating up projectiles in the first 0.5s after starting Nullify now grants 5x the value
- ^less effective for passively charging up, but more rewarding on well timed counters
- Pummel unlock requirement changed from 5 slams to 3
- Pummel base damage: 1000% > 1250%
- ^Brutalize goes up to 1500% based on fall distance, so this is just addressing that damage disparity
- Pummel now applies heavy force to its target, guaranteeing most things get hit by the following shockwave
- Empowered Pummel new effect: no longer doubles damage, but instead hits the enemy with a barrage of punches
- Updated default skin model once again
- Various forms of visual and audio polish
- Fixed incorrect healing amount in special descriptions

`1.3.5`
- Physical Prowess leaps now force a sprint- this removes the necessity to be sprinting beforehand to get full value out of the leap
- Fixed Blood Rush overlay expiring near instantly

`1.3.4`
- Added new unlockable alt special
- Empowered punch now deals double damage
- Twisted Mutation now refreshes when grabbing something with Brutalize- means you can blink, grab, blink again for more style points
- Updated some VFX for the void skins
- Fixed issues resulting from grabbing flying bosses with Brutalize
- Fixed Hopoo Feather doing nothing with Twisted Mutation- now consumes the extra jumps before using up the blink
- Fixed duplicate passive in character select

`1.3.3`
- Nullify beam duration no longer scales with attack speed; tick rate still does though (maybe op who knows)
- Rewrote Brutalize punch logic to fix multiplayer weirdness
- Added config to disable the boss grab from empowered Brutalize- recommended until the collision issues are fixed, or if you find it grief in multiplayer
- Fixed Coagulate not playing its animation for multiplayer clients

`1.3.2`
- Base armor: -10 > 0
- Fixed Nullify starting off at full charge if it had previously absorbed projectiles

`1.3.1`
- whoops forgot to remove the network testing hook

`1.3.0`

# Changes
- Networked EVERYTHING! Every single part of the character should now work properly on every client! Huge thanks to Tsweep and Moffein for helping with this :-)
- Added alternate primary: Dismantle - Three hit combo with bonus damage and stun on the third hit
- Physical Prowess wall leaps now scale with movement speed, but base distance has been reduced
- Added new mastery skin and moved the current one to cursed config
- Finally added unique emotes
- Fixed Coagulate hop distance not scaling properly

# Buffs
- Hack Hold Stance damage multiplier increased from 4x to 5x, special animation added for max damage swings
- Blood Well activation healing now counts as healing instead of regen, letting it benefit from the appropriate items
- Blood Well no longer drains while charging a wall jump- should give it a little more niche use in combat
- Brutalize punch shockwave damage: 400% > 1000%
- Brutalize new effect: Can grab bosses when empowered
- Brutalize now empowers Wandering Vagrant orbs when grabbed, greatly increasing blast radius and damage
- Empowered Nullify charge speed increased

# Nerfs
- Base armor: 20 > -10
- Base health regen: 2.5/s > 0.5/s
- Hack damage: 270% > 230%
- Cleave damage: 600% > 500%
- Blood Well activation healing: 100% max hp > 75% missing hp
- Brutalize heal on kill: 15% max hp > 10% max hp
- Brutalize drag damage over time: 300%/s > 100%/s
- Brutalize now deals half damage for each repeated slam on the same enemy (in the same grab)

`1.2.4`
- Fixed a gamebreaking bug caused by using Brutalize on an enemy with the Witch's Ring item from Aetherium (thanks yarrowyeen for finding the cause!)

`1.2.3`
- Finally polished the stupid readme, god why did that take two whole ass hours
- Slightly increased size of melee hitboxes
- Twisted Mutation blink distance slightly increased
- Twisted Mutation blink now refreshes on melee hits and has subtle VFX showing when a blink is available
- Grabbing enemies with Brutalize now sets their damage stat to 0 to prevent blazing elites from killing you
- Nullify charge gain per projectile slightly decreased
- Added config to stop Nullify from eating allied projectiles
- Added config to let you cling to walls permanently
- Slight model improvements
- Updated a handful of item displays
- Fixed Brutalize punch doing way too much damage
- Fixed Hopoo Feather interaction not actually working

`1.2.2`
- Brutalize can now be used on bosses to perform a punch with a shockwave hitting enemies directly behind
- Hack damage: 320% > 270%
- Hack bonus attack speed while empowered slightly lowered
- ^Brutalize greatly raised his kill speed on bosses so some nerfing was needed (still real strong)
- Hopoo Feathers now allow you to perform walljumps in the air
- Attempted to network Blood Well heal but no guarantees
- yes i know the skins suck they're just shitposts i'll make proper skins for him eventually

`1.2.1`
- Apparently this never existed?

`1.2.0`
- Added a new unlockable alternate passive
- Added a new unlockable skin
- Added a Badass Mode config option (warning: Far Too Epic!!!!)
- Reworked Eldritch Blast and renamed it to Nullify, added unlock condition
- Moved Blood Well's description from primary skill to its own passive slot for clarity
- Blood Well heal amount: 75% max health > 100% max health
- Brutalize heal amount: 30% max health > 15% max health
- ^less on demand healing, more reward on the heal you actually have to work for
- Blood Rush duration slightly increased
- Coagulate's empowered explosion now always deals max damage regardless of how much meter is left
- ^makes it a viable finisher to Blood Rush instead of being all around awkward
- Added some config for the Blood Well HUD
- Attack hitstop now applies to slash effects as well
- Fixed Brutalize drag damage being 80% instead of the intended 800%
- Fixed slash effects not being networked- thanks tsweep :-)
- Fixed some other misc bugs idek

`1.1.2`
- Updated a handful of animations, no more pod :-)
- Skill icons now get a cool overlay when you're in Blood Rush
- Coagulate new effect: When empowered, now explodes for up to 2400% damage based on how much blood is expended
- Greatly increased the drag damage from empowered Brutalize, making it rewarding beyond purely feeling cool
- Fixed weird Brutalize interactions with certain flying enemies

`1.1.1`
- Bugfix

`1.1.0`
- Added mastery skin complete with custom VFX and SFX!!
- Hack damage 390% > 320% (this carries over to the hold stance too)
- Cleave damage 750% > 600%
- ^damage values haven't been changed once so now that he's out it's time to hit him with the balance hammer
- Moved Blood Well gauge onto the crosshair for better visibility
- Added some extra animations for Cleave
- Updated Blood Rush VFX
- Updated Brutalize skill icon
- Attempted to network Coagulate and Blood Well
- Fixed delays on a bunch of VFX
- Fixed broken VFX texture in lobby

`1.0.5`
- Fixed weird Shuriken procs

`1.0.4`
- Fixed funny grab bug

`1.0.3`
- Greatly increased forward lunge force on Cleave
- Updated Cleave VFX, SFX and animation
- Enemies grabbed by empowered Brutalize no longer die until the end of the drag
- Updated Brutalize blood VFX to be more noticeable
- Attempted to fix Brutalize sometimes not doing max damage or even hitting at all
- Added work-in-progress alternate utility skill, Eldritch Blast
- ^this is way more active and also way more overpowered so expect nerfs? maybe? who knows

`1.0.2`
- Removed wall jump limit heheheha may revert this, who knows, go crazy
- Lowered secondary animation lockout duration
- Increased forward lunge force of grounded secondary

`1.0.1`
- Timer for charge jumps increased from 0.5s > 0.65s, should add a bit more of a window to aim
- Fixed broken health scaling

`1.0.0`
- Initial release