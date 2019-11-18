import { createStyles } from "AppStyles";

export const map = createStyles({
	mapCover: {
		background: "url('/Maps/Placeholder-480x240.jpg') no-repeat transparent",
		backgroundSize: "cover",
		position: "relative",
		overflow: "hidden",

		"&.KF-Farmhouse": { backgroundImage: "url('/Maps/KF-Farmhouse-480x240.jpg');" },
		"&.KF-BlackForest": { backgroundImage: "url('/Maps/KF-BlackForest-480x240.jpg');" },
		"&.KF-Outpost": { backgroundImage: "url('/Maps/KF-Outpost-480x240.jpg');" },
		"&.KF-Prison": { backgroundImage: "url('/Maps/KF-Prison-480x240.jpg');" },
		"&.KF-BioticsLab": { backgroundImage: "url('/Maps/KF-BioticsLab-480x240.jpg');" },
		"&.KF-BurningParis": { backgroundImage: "url('/Maps/KF-BurningParis-480x240.jpg');" },
		"&.KF-HostileGrounds": { backgroundImage: "url('/Maps/KF-HostileGrounds-480x240.jpg');" },
		"&.KF-ZedLanding": { backgroundImage: "url('/Maps/KF-ZedLanding-480x240.jpg');" },
		"&.KF-TheDescent": { backgroundImage: "url('/Maps/KF-TheDescent-480x240.jpg');" },
		"&.KF-ContainmentStation": { backgroundImage: "url('/Maps/KF-ContainmentStation-480x240.jpg');" },
		"&.KF-Nuked": { backgroundImage: "url('/Maps/KF-Nuked-480x240.jpg');" },
		"&.KF-Catacombs": { backgroundImage: "url('/Maps/KF-Catacombs-480x240.jpg');" },
		"&.KF-EvacuationPoint": { backgroundImage: "url('/Maps/KF-EvacuationPoint-480x240.jpg');" },
		"&.KF-VolterManor": { backgroundImage: "url('/Maps/KF-VolterManor-480x240.jpg');" },
		"&.KF-InfernalRealm": { backgroundImage: "url('/Maps/KF-InfernalRealm-480x240.jpg');" },
		"&.KF-Nightmare": { backgroundImage: "url('/Maps/KF-Nightmare-480x240.jpg');" },
		"&.KF-TragicKingdom": { backgroundImage: "url('/Maps/KF-TragicKingdom-480x240.jpg');" },
		"&.KF-KrampusLair": { backgroundImage: "url('/Maps/KF-KrampusLair-480x240.jpg');" },
		"&.KF-Lockdown": { backgroundImage: "url('/Maps/KF-Lockdown-480x240.jpg');" },
		"&.KF-MonsterBall": { backgroundImage: "url('/Maps/KF-MonsterBall-480x240.jpg');" },
		"&.KF-PowerCore_Holdout": { backgroundImage: "url('/Maps/KF-PowerCore-480x240.jpg');" },
		"&.KF-Airship": { backgroundImage: "url('/Maps/KF-Airship-480x240.jpg');" },
		"&.KF-ShoppingSpree": { backgroundImage: "url('/Maps/KF-ShoppingSpree-480x240.jpg');" },
		"&.KF-SantasWorkshop": { backgroundImage: "url('/Maps/KF-SantasWorkshop-480x240.jpg');" },
		"&.KF-Spillway": { backgroundImage: "url('/Maps/KF-Spillway-480x240.jpg');" },
		"&.KF-SteamFortress": { backgroundImage: "url('/Maps/KF-SteamFortress-480x240.jpg');" },
		"&.KF-AshwoodAsylum": { backgroundImage: "url('/Maps/KF-AshwoodAsylum-480x240.jpg');" },
		"&.KF-Sanitarium": { backgroundImage: "url('/Maps/KF-Sanitarium-480x240.jpg');" },
	},
}).attach().classes;
