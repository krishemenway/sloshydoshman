import { ResultOf } from "CommonDataStructures/ResultOf";
import { PlayerViewModel } from "Server";
import { Observable, Subscription } from "knockout";
import * as HashChange from "../KnockoutHelpers/HashchangeExtender";
import * as NumberWithCommas from "KnockoutHelpers/NumberWithCommasBindingHandler";
import * as PlayerPerkStatistics from "PlayerView/PlayerPerkStatisticsComponent";
import * as PlayerMapStatistics from "PlayerView/PlayerMapStatisticsComponent";
import * as PlayedGameList from "PlayerView/PlayedGameListComponent";
import * as ko from "knockout";
import { layout, margin, text, textColor, padding, redHandleContainer } from "AppStyles";

class PlayerProfileViewModel {
	constructor() {
		this.PlayerViewModel = ko.observable(null);
		this.SteamId = HashChange.CreateObservable("SteamId", "");
		this.SteamIdSubscription = this.SteamId.subscribe(() => this.InitializePlayer());
		this.InitializePlayer();
	}

	public dispose() {
		this.SteamIdSubscription.dispose();
	}

	public OnMapSelected = (map: string) => {
		console.log(map);
	}

	private InitializePlayer = () => {
		$.get(`/webapi/players/profile?steamId=${this.SteamId()}`).done((response: ResultOf<PlayerViewModel>) => this.PlayerViewModel(response.Data));
	}

	public PlayerViewModel: Observable<PlayerViewModel|null>;
	public SteamId: Observable<string>;
	private SteamIdSubscription: Subscription;
}

export var Name : string = "Player";
ko.components.register(Name, {
	viewModel: PlayerProfileViewModel,
	template: `
		<div class="${layout.flexRow}" data-bind="with: PlayerViewModel">
			<div class="${layout.flexEvenDistribution} ${margin.rightHalf}" style="max-width: 55%">
				<div class="${redHandleContainer.container} ${margin.bottom}">
					<div class="${text.font48} ${textColor.white} ${text.bold}" data-bind="html: UserName" />

					<div class="${textColor.white}">
						Steam ID: <span class="${text.bold} ${textColor.gray}" data-bind="text: SteamId" />
					</div>
				</div>

				<div class="${redHandleContainer.container} ${margin.bottom} ${textColor.white}">
					<div class="${redHandleContainer.header} ${text.center} ${margin.bottom}">recent games</div>
					<div data-bind="component: {name: '${PlayedGameList.ComponentName}', params: {Games: AllGames}}"></div>
				</div>

				<div class="${textColor.white} ${text.center} ${margin.bottom}">
					<div class="${text.center} ${layout.width50} ${layout.inlineBlock} ${padding.rightHalf}">
						<div class="${redHandleContainer.container}">
							<div class="${text.font48} ${text.light}" data-bind="${NumberWithCommas.Name}: TotalKills" />
							<div class="${text.font14} ${textColor.gray}">Total Kills</div>
						</div>
					</div>

					<div class="${text.center} ${layout.width50} ${layout.inlineBlock} ${padding.leftHalf}">
						<div class="${redHandleContainer.container}">
							<div class="${text.font48} ${text.light}" data-bind="${NumberWithCommas.Name}: TotalGames" />
							<div class="${text.font14} ${textColor.gray}">Total Games</div>
						</div>
					</div>
				</div>

				<div class="${redHandleContainer.container}" data-bind="component: {name: '${PlayerPerkStatistics.ComponentName}', params: $component.PlayerViewModel}" />
			</div>

			<div class="${layout.flexEvenDistribution} ${redHandleContainer.container} ${margin.leftHalf} ${textColor.white}">
				<div class="${redHandleContainer.header} ${text.center} ${margin.bottomDouble}">maps</div>
				<div data-bind="component: {name: '${PlayerMapStatistics.ComponentName}', params: {PlayerViewModel: $component.PlayerViewModel}}" />
			</div>
		</div>`,
});