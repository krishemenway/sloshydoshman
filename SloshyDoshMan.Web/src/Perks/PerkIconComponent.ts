import * as ko from "knockout";
import { createStyles } from "AppStyles";

var PerkIcon : string = "PerkIcon";
export function PerkIconComponent(perk: string) {
	return `component: { name: '${PerkIcon}', params: { Perk: ${perk} } }`;
}

interface PerkIconParams {
	Perk: string;
}

class PerkIconViewModel {
	constructor(params: PerkIconParams) {
		this.Perk = params.Perk;
		this.PerkBackgroundImage = `url('/Perks/${params.Perk}-128')`;
	}

	public Perk: string;
	public PerkBackgroundImage: string;
}

const styles = createStyles({
	perkIcon: {
		display: "inline-block",
		width: "32px",
		height: "32px",
		backgroundSize: "100% 100%",
	},
}).attach().classes;

ko.components.register(PerkIcon, {
	viewModel: PerkIconViewModel,
	template: `<span class="${styles.perkIcon}" data-bind="style: { backgroundImage: $component.PerkBackgroundImage }, attr: {title: Perk}" />`,
});
