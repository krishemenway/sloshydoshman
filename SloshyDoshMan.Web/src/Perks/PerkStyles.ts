import { createStyles } from "AppStyles";

export const perk = createStyles({
	perkIcon: { 
		"display": "inline-block",

		"&.Berserker": { "background-image": "url('/Perks/Berserker-128.png')" },
		"&.Commando": { "background-image": "url('/Perks/Commando-128.png')" },
		"&.Demolitionist": { "background-image": "url('/Perks/Demolitionist-128.png')" },
		"&.Medic": { "background-image": "url('/Perks/Field Medic-128.png')" },
		"&.Firebug": { "background-image": "url('/Perks/Firebug-128.png')" },
		"&.Gunslinger": { "background-image": "url('/Perks/Gunslinger-128.png')" },
		"&.Sharpshooter": { "background-image": "url('/Perks/Sharpshooter-128.png')" },
		"&.Support": { "background-image": "url('/Perks/Support-128.png')" },
		"&.Survivalist": { "background-image": "url('/Perks/Survivalist-128.png')" },
		"&.SWAT": { "background-image": "url('/Perks/SWAT-128.png')" },
	},
	width32: { "width": "32px", "height": "32px", "background-size": "32px 32px" },
	width128: { "width": "128px", "height": "128px", "background-size": "128px 128px" },
}).attach().classes;
