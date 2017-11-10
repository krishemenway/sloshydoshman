import * as ko from "knockout";

let componentLoader : KnockoutComponentTypes.Loader = {
	loadTemplate: function(name: string, templateConfig: any, callback: (result: Node[]|null) => void) {
		if (ko.components.defaultLoader.loadTemplate === undefined) {
			throw "Something is wrong with knockout";
		}

		if(typeof templateConfig === "string") {
			ko.components.defaultLoader.loadTemplate(name, templateConfig.replace(/[\t\n]/g, ''), callback);
		} else {
			ko.components.defaultLoader.loadTemplate(name, templateConfig, callback);
		}
	}
}

export function AddComponentCleaner() {
	ko.components.loaders.unshift(componentLoader);
}
