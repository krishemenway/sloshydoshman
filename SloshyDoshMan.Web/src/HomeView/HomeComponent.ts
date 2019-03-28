import * as ko from "knockout";
import { margin, text, textColor } from "../AppStyles";
import { ServerRecentWinsComponent } from "HomeView/ServerRecentWinsComponent";
import { ServerRecentGamesComponent } from "HomeView/ServerRecentGamesComponent";
import { ServerStatisticsComponent } from "HomeView/ServerStatisticsComponent";
import { PlayerSearchComponent } from "HomeView/PlayerSearchComponent";

export const HomeComponentName : string = "Home";
export function HomeComponent() {
	return `component: {name: '${HomeComponentName}'}`;
}

class HomeViewModel {
	constructor(params?: any) { }
}

ko.components.register(HomeComponentName, {
	viewModel: HomeViewModel,
	template: `
		<div>
			<div class="${margin.bottom}" data-bind="${ServerRecentWinsComponent()}" />
			<div class="${margin.bottom}" data-bind="${PlayerSearchComponent()}" />
			<div class="${margin.bottom}" data-bind="${ServerRecentGamesComponent()}" />
			<div class="${margin.bottom}" data-bind="${ServerStatisticsComponent()}" />
			<div class="${margin.bottom} ${text.center} ${textColor.white} ${text.font14} ${text.smallCaps}">
				<a href="https://github.com/krishemenway/sloshydoshman">source code</>
			</div>
		</div>`,
});
