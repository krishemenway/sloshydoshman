import * as ko from "knockout";
import { text, margin, textColor, layout, redHandleContainer } from "AppStyles";
import { Dictionary } from "CommonDataStructures/Dictionary";
import { CreateHashChangeObservable, SetState } from "KnockoutHelpers/HashchangeExtender";
import { HomeComponentName } from "HomeView/HomeComponent";
import * as ComponentCleaner from "KnockoutHelpers/ComponentCleaner";

export var ComponentName : string = "App";

export function GoToView(view: string, data?: Dictionary<string>) {
	let state : Dictionary<string> = {'v': view};

	if(!!data) {
		Object.keys(data).forEach((key) => {
			state[key] = data[key];
		});
	}

	SetState(state);
}

export class AppViewModel {
	constructor(params?: any) {
		this.CurrentView = CreateHashChangeObservable("v", HomeComponentName);
	}

	public CurrentView: ko.Observable<string>;
}

ko.components.register(ComponentName, {
	viewModel: AppViewModel,
	template: `
		<div class="${layout.centerLayout1000}">
			<div class="${margin.all} ${text.font14} ${redHandleContainer.container} ${textColor.white}">
				This KF2 stats system is a work in progress. It is based on the limited data that is available through the web admin interface. If you wish to contact me about this project, find me on steam: linkstate or email: thelinkstate@gmail.com.
			</div>

			<!-- ko if: CurrentView -->
			<div data-bind="component: {name: CurrentView}" />
			<!-- /ko -->

			<!-- ko ifnot: CurrentView -->
			<div class="${redHandleContainer.container} ${text.center}">loading</div>
			<!-- /ko -->
		</div>`
});

ComponentCleaner.AddComponentCleaner();
