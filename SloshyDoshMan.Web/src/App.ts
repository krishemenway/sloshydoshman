import {Dictionary} from "CommonDataStructures/Dictionary";
import {Observable} from "knockout";
import { text, margin, textColor, layout } from "AppStyles";
import * as HashChange from "KnockoutHelpers/HashchangeExtender";
import * as ko from "knockout";
import * as HomeView from "HomeView/HomeComponent";
import * as ComponentCleaner from "KnockoutHelpers/ComponentCleaner";

export var ComponentName : string = "App";

export function GoToView(view: string, data?: Dictionary<string>) {
	let state : Dictionary<string> = {'v': view};

	if(!!data) {
		Object.keys(data).forEach((key) => {
			state[key] = data[key];
		});
	}

	HashChange.SetState(state);
}

export class AppViewModel {
	constructor(params?: any) {
		this.CurrentView = HashChange.CreateObservable("v", HomeView.Name);
	}

	public CurrentView: Observable<string>;
}

ko.components.register(ComponentName, {
	viewModel: AppViewModel,
	template: `
		<div class="center-layout-1000">
			<div class="${margin.all} ${text.font14} ${layout.redHandleContainer} ${textColor.white}">
				This KF2 stats system is a work in progress. It is based on the limited data that is available through the web admin interface. If you wish to contact me about this project, find me on steam: linkstate or email: thelinkstate@gmail.com.
			</div>
			<!-- ko if: CurrentView -->
			<div data-bind="component: {name: CurrentView}" />
			<!-- /ko -->
			<!-- ko ifnot: CurrentView -->
			<div class="${layout.redHandleContainer} ${text.center}">loading</div>
			<!-- /ko -->
		</div>`
});

ComponentCleaner.AddComponentCleaner();
