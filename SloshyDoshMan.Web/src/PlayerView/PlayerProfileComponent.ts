import * as ko from "knockout";
import { layout, margin, text, textColor, padding, redHandleContainer, events } from "AppStyles";
import { ResultOf } from "CommonDataStructures/ResultOf";
import { PlayerViewModel } from "Server";
import { CreateHashChangeObservable } from "KnockoutHelpers/HashchangeExtender";
import { NumberWithCommas } from "KnockoutHelpers/NumberWithCommasBindingHandler";
import { PlayerPerkStatisticsComponent } from "PlayerView/PlayerPerkStatisticsComponent";
import { PlayedGameListComponent } from "PlayerView/PlayedGameListComponent";
import { PlayerMapStatisticsComponent } from "PlayerView/PlayerMapStatisticsComponent";
import { GoToView } from "App";

var Name : string = "Player";
export function GoToPlayerProfile(steamId: string) {
	GoToView(Name, { SteamId: steamId });
}

class PlayerProfileViewModel {
	constructor() {
		this.PlayerViewModel = ko.observable(null);
		this.SteamId = CreateHashChangeObservable("SteamId", "");

		this.LoadingPlayerInformation = ko.observable(false);
		this.LoadPlayerInformationFailureMessage = ko.observable("");

		this.SteamIdSubscription = this.SteamId.subscribe(() => this.LoadPlayerInformation());
		this.LoadPlayerInformation();
	}

	public dispose() {
		this.SteamIdSubscription.dispose();
	}

	public LoadPlayerInformation = () => {
		this.HandlePlayerLoadedFailure("");
		this.LoadingPlayerInformation(true);

		$.get(`/webapi/players/profile?steamId=${this.SteamId()}`)
		 .done(this.HandlePlayerLoadedResponse)
		 .fail(() => this.HandlePlayerLoadedFailure("Something went wrong loading the player information. Please try again later."))
		 .always(() => this.LoadingPlayerInformation(false));
	}

	private HandlePlayerLoadedResponse = (response: ResultOf<PlayerViewModel>) => {
		if (response.Success) {
			this.PlayerViewModel(response.Data);
		} else {
			this.HandlePlayerLoadedFailure(response.ErrorMessage);
		}
	}

	private HandlePlayerLoadedFailure = (failureMessage: string) => {
		this.LoadPlayerInformationFailureMessage(failureMessage);
	}

	public PlayerViewModel: ko.Observable<PlayerViewModel|null>;
	public SteamId: ko.Observable<string>;

	public LoadingPlayerInformation: ko.Observable<boolean>;
	public LoadPlayerInformationFailureMessage: ko.Observable<string>;

	private SteamIdSubscription: ko.Subscription;
}

ko.components.register(Name, {
	viewModel: PlayerProfileViewModel,
	template: `
		<div class="${layout.flexRow}">
			<!-- ko if: !!LoadPlayerInformationFailureMessage() -->
			<div class="${redHandleContainer.container} ${layout.width100} ${text.center} ${textColor.white}">
				<span data-bind="text: LoadPlayerInformationFailureMessage" />
				<button class="${events.clickable} ${margin.left} ${padding.half}" data-bind="click: LoadPlayerInformation">Try Again?</button>
			</div>
			<!-- /ko -->

			<!-- ko with: PlayerViewModel -->
			<div class="${layout.flexEvenDistribution} ${margin.rightHalf}" style="max-width: 55%">
				<div class="${redHandleContainer.container} ${margin.bottom}">
					<div class="${text.font48} ${textColor.white} ${text.bold}" data-bind="html: UserName" />

					<div class="${textColor.white}">
						Steam ID: <span class="${text.bold} ${textColor.gray}" data-bind="text: SteamId" />
					</div>
				</div>

				<div class="${redHandleContainer.container} ${margin.bottom} ${textColor.white}">
					<div class="${redHandleContainer.header} ${text.center} ${margin.bottom}">recent games</div>
					<div data-bind="${PlayedGameListComponent("AllGames")}" />
				</div>

				<div class="${textColor.white} ${text.center} ${margin.bottom}">
					<div class="${text.center} ${layout.width50} ${layout.inlineBlock} ${padding.rightHalf}">
						<div class="${redHandleContainer.container}">
							<div class="${text.font48} ${text.light}" data-bind="${NumberWithCommas("TotalKills")}" />
							<div class="${text.font14} ${textColor.gray}">Total Kills</div>
						</div>
					</div>

					<div class="${text.center} ${layout.width50} ${layout.inlineBlock} ${padding.leftHalf}">
						<div class="${redHandleContainer.container}">
							<div class="${text.font48} ${text.light} ${text.right}" data-bind="${NumberWithCommas("TotalGames")}" />
							<div class="${text.font14} ${textColor.gray} ${text.right}">Total Games</div>
						</div>
					</div>
				</div>

				<div class="${redHandleContainer.container}" data-bind="${PlayerPerkStatisticsComponent("$component.PlayerViewModel")}" />
			</div>

			<div class="${layout.flexEvenDistribution} ${redHandleContainer.container} ${margin.leftHalf} ${textColor.white}">
				<div class="${redHandleContainer.header} ${text.center} ${margin.bottomDouble}">maps</div>
				<div data-bind="${PlayerMapStatisticsComponent("$component.PlayerViewModel")}" />
			</div>
			<!-- /ko -->
		</div>`,
});