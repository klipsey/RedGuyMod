# Driver
- Adds the Driver, an original character based on the Driver from the film Drive (2011)
- Fully functional base kit with several weapon pickups that give him new attacks
- Has item displays, an unlockable mastery skin and is fully multiplayer compatible
- Configurable stats and a few other things to mess around with
- Full Risk of Options support for all configuration options
- Many more weapons planned!!!!

[![](https://raw.githubusercontent.com/ArcPh1r3/DriverMod/main/Release/FuckShit/screen1.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/DriverMod/main/Release/FuckShit/screen2.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/DriverMod/main/Release/FuckShit/screen3.png)]()

[![](https://raw.githubusercontent.com/ArcPh1r3/DriverMod/main/DriverUnityProject/Assets/Driver/Icons/texDriverIcon.png)]()

To share feedback, report bugs, or offer suggestions feel free to DM @braindead_ape on Discord

___

# Unlock

[![](https://raw.githubusercontent.com/ArcPh1r3/DriverMod/main/Release/FuckShit/unlock.png)]()

___

# Skills

## Passive
Enemies drop weapon crates that change your skills for 8 seconds, starting when you first attack with it

[![](https://github.com/ArcPh1r3/DriverMod/blob/3f2ba4c0dd4ff37efa4f28288620f36525f289f7/Release/FuckShit/passive.gif?raw=true)]()

## Primary
Shoot gun

[![](https://github.com/ArcPh1r3/DriverMod/blob/3f2ba4c0dd4ff37efa4f28288620f36525f289f7/Release/FuckShit/primary.gif?raw=true)]()

## Secondary
Aim gun and charge a high damage shot

[![](https://github.com/ArcPh1r3/DriverMod/blob/3f2ba4c0dd4ff37efa4f28288620f36525f289f7/Release/FuckShit/secondary.gif?raw=true)]()

## Utility
Slide

[![](https://github.com/ArcPh1r3/DriverMod/blob/3f2ba4c0dd4ff37efa4f28288620f36525f289f7/Release/FuckShit/utility.gif?raw=true)]()

## Special
Stun grenade

[![](https://github.com/ArcPh1r3/DriverMod/blob/3f2ba4c0dd4ff37efa4f28288620f36525f289f7/Release/FuckShit/special.gif?raw=true)]()

___

## Donations
If you enjoy my work and would like to support me, you can donate to my Ko-fi: https://ko-fi.com/robdev

## Credits
rob - Mostly everything

TheTimesweeper - Maintaining everything in my absence, making it super easy to get back on my feet, lots of code help and feedback

Moffein - Also maintained things :-) Riot Shotgun SFX

Swuff - Code help, feedback, networking help

Bog - Code for Shuriken interaction fix

Hyperinvox - Briefcase model

Bruh - Riot Shotgun model

Glasus - Grand Mastery skin concept


## Future Plans
- Many more weapons (rocket launcher, dual uzis, railgun, minigun, etc.)
- Alternate skills
- Ancient Scepter support
- Better item displays
- Other things 100% decided on pure whimsy
- Proper skill icons?
- More skins

___

## Changelog

`1.2.1`
- Added weapon: Sniper Rifle (thanks to Swuff for the weak point targeting implementation)
- Added a new unlockable skin (thanks Glasus for the challenge idea)
- Flashbang damage: 400% > 650%, animation slightly sped up
- Tweaked kickback on all shotguns
- Bazooka minimum damage: 400% > 600%, charge time lowered and lockout before you can release an uncharged shot decreased
- Updated Grand Mastery skin, adding more detail and polish
- Fixed weapon duration meter being visible with no weapon equipped
- Fixed Eclipse levels not going up

`1.2.0`
- Added Grand Mastery skin!! huge thanks to Glasus for the design
- Added weapon: Bazooka
- Added an unlockable alternate special- Flashbang bridges a pretty crucial gap in his kit so hopefully it's not a straight upgrade
- Machine Gun primary damage: 190% > 200%, fire rate slightly increased
- Heavy Machine Gun primary damage: 240% > 210%, fire rate increased, shots now ignore armor (more damage to bosses)
- Changed a handful of item displays
- Enemies marked by Starstorm 2's Relic of Termination are now guaranteed to drop a Rocket Launcher
- All shotguns now have varying degrees of backwards force when fired
- Updated models for Slug Shotgun and Rocket Launcher
- Temporarily removed functionality from weapon duration configs and reverted Rocket Launcher to 8 seconds

`1.1.2`
- Pocket ICBM now works on Rocket Launcher. This has to be hardcoded into the skill state apparently
- Rocket Launcher primary damage: 800% > 1000%
- Rocket Launcher duration: 8s > 20s - Considering it only drops from bosses, at the end of the teleporter event for pretty much the whole first loop, it can get away with lasting a while
- Added weapon: Slug Shotgun
- Added weapon: Heavy Machine Gun
- Added config options to enable or disable weapons
- Fixed weapon pickups dropping for the enemy teams, preventing player Drivers from picking them up
- Fixed Pistol and Machine Gun spread patterns
- Fixed Simulacrum boss waves dropping Rocket Launchers from every enemy

`1.1.1`
- Rocket Launcher primary damage: 600% > 800%, fire rate increased
- Shotgun primary damage: 6x190% > 8x190%, removed pierce
- Added weapon: Riot Shotgun, deals 6x140% with 0.75 proc coefficient but pierces (thanks bruh for the model =))
- Removed level scaling and nerfed weapon drop pity modifier a little, last update made them drop just a bit too frequently
- Added a custom pickup model for legendary tier weapons
- Added a config option to make Driver call out his weapon pickups- enable this in multiplayer to make people mad
- Added a config option to use the old crate pickups if you for some reason liked those more
- Added cursed config

`1.1.0`
- Added weapon: Rocket Launcher, guaranteed drop from bosses
- Picking up Bandolier drops now resets your weapon timer
- Replaced goofy weapon drop crates with a cool looking briefcase (old ones will be a config at some point)
- Pistol now has some spread when firing without Focus
- Machine Gun spread reworked for better accuracy when burst firing
- Rewrote entire weapon backend, allowing new weapons to be easily added (even from external mods) and streamlining the whole development process
- Made mastery skin 3% more epic
- Fixed the big guy, elite, boss drop rate tweaks from 1.0.3 not working, please don't ask why this wasn't working
- Fixed Machine Gun's secondary cancelling before the projectile was fired if you had some attack speed

`1.0.5`
- Fixed ragdoll; the incredibly handsome developer of this mod never dies and so never realized it was broken

`1.0.4`
- Buffed Pistol's fire rate
- Shooting with Focus no longer has damage falloff
- Machine Gun no longer has falloff
- Shotgun now has falloff
- Weapon drops now have a pity system, chance goes up with each kill until one drops and then the pity modifier resets
- Weapon drops no longer require a Driver to land the killing blow- the only requirement now is a Driver on the player team
- Fixed Combat Slide consuming all charges of Hardlight Afterburner instantly

`1.0.3`
- Weapon timer won't start until your first attack with the weapon now
- Tweaked weapon drop rates
-- Champions (beetle guard, templar, etc. big guys) now have double the chance to drop a weapon
-- Elites have 50% minimum chance to drop one, and can still scale up past that threshold
-- Bosses are guaranteed to drop one- when higher tier weapons are added, they will drop these as well
-- Drop chance is halved when playing on Swarms

`1.0.2`
- Synced gun pickups, but for now only the host is able to see what's in the crate (until i can figure out this stupid code)
- Fixed non host players being unable to pick up guns
- Fixed non host players throwing a Captain airstrike instead of a flashbang

`1.0.1`
- Fixed gun pickups being permanent- when commenting out the gun test code for release, I accidentally hit the code that swaps back to the pistol as well, whoops

`1.0.0`
- Initial release