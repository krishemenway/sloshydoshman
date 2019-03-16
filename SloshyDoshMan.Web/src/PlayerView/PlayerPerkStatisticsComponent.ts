import {PlayerViewModel} from "Server";
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import {Observable} from "knockout";
import * as ko from "knockout";

export var ComponentName = "PlayerPerkStatistics";

class PlayerPerkStatisticsViewModel {
	constructor(params: Observable<PlayerViewModel>) {
		this.PlayerViewModel = params;
	}

	public PlayerViewModel: Observable<PlayerViewModel>;
}

ko.components.register(ComponentName, {
	viewModel: PlayerPerkStatisticsViewModel,
	template: `
		<div class="perk-statistics text-white" data-bind="with: PlayerViewModel">

			<!-- ko foreach: PerkStatistics -->
				<div class="margin-bottom inline-block width-50">
					<div class="margin-bottom-half">
						<span class="vert-middle perk-icon width-32" data-bind="css: Perk, attr: {title: Perk}"></span>
						<span class="vert-middle margin-left font-16 small-caps" data-bind="text: Perk" />
					</div>

					<div class="flex-row" style="margin-left: 42px">
						<div class="flex-even-distribution text-center">
							<div class="font-24 gray-e8 font-light" data-bind="${NumberWithCommas.Name}: TotalKills"></div>
							<div class="font-12 text-gray">Kills</div>
						</div>

						<div class="flex-even-distribution text-center">
							<div class="font-24 gray-e8 font-light text-right" data-bind="${NumberWithCommas.Name}: TotalWavesPlayed"></div>
							<div class="font-12 text-gray text-right">Waves Played</div>
						</div>
					</div>

					<!-- ko if: $index() < $parent.PerkStatistics.length - 2 -->
					<div style="margin-left: 32px"><div class="horz-rule margin-top block-center" /></div>
					<!-- /ko -->
				</div>
			<!-- /ko -->

		</div>`,
});