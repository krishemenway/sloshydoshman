import * as ko from "knockout";
import { textColor, margin, layout, text } from "AppStyles";
import { perk } from "Perks/PerkStyles";
import { PlayerViewModel } from "Server";
import { NumberWithCommas } from "KnockoutHelpers/NumberWithCommasBindingHandler";

export var ComponentName = "PlayerPerkStatistics";
export function PlayerPerkStatisticsComponent(playerViewModelParameter: string) {
	return `component: {name: '${ComponentName}', params: ${playerViewModelParameter}}`;
}

class PlayerPerkStatisticsViewModel {
	constructor(playerViewModel: ko.Observable<PlayerViewModel>) {
		this.PlayerViewModel = playerViewModel;
	}

	public PlayerViewModel: ko.Observable<PlayerViewModel>;
}

ko.components.register(ComponentName, {
	viewModel: PlayerPerkStatisticsViewModel,
	template: `
		<div class="${textColor.white}" data-bind="with: PlayerViewModel">

			<!-- ko foreach: PerkStatistics -->
				<div class="${margin.bottom} ${layout.inlineBlock} ${layout.width50}">
					<div class="${margin.bottomHalf}">
						<span class="${layout.vertMiddle} ${perk.perkIcon} ${perk.width32}" data-bind="css: Perk, attr: {title: Perk}"></span>
						<span class="${layout.vertMiddle} ${margin.left} ${text.font16} ${text.smallCaps}" data-bind="text: Perk" />
					</div>

					<div class="${layout.flexRow}" style="margin-left: 42px; margin-right: 8px;">
						<div class="${layout.flexEvenDistribution} ${text.center}">
							<div class="${text.font24} ${textColor.graye8} ${text.light}" data-bind="${NumberWithCommas("TotalKills")}"></div>
							<div class="${text.font12} ${textColor.gray}">Kills</div>
						</div>

						<div class="${layout.flexEvenDistribution} ${text.center}">
							<div class="${text.font24} ${textColor.graye8} ${text.light} ${text.right}" data-bind="${NumberWithCommas("TotalWavesPlayed")}"></div>
							<div class="${text.font12} ${textColor.gray} ${text.right}">Waves Played</div>
						</div>
					</div>

					<!-- ko if: $index() < $parent.PerkStatistics.length - 2 -->
					<div style="margin-left: 32px">
						<div class="${layout.horzRule} ${margin.top} ${layout.blockCenter}" />
					</div>
					<!-- /ko -->
				</div>
			<!-- /ko -->

		</div>`,
});