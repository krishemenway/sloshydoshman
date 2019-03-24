import {components} from "knockout";
import * as ko from "knockout";

let componentLoader : components.Loader = {
	loadTemplate: function(name: string, templateConfig: any, callback: (result: Node[]|null) => void) {
		if (ko.components.loaders[1].loadTemplate === undefined) {
			throw "Something is wrong with knockout";
		}

		if(typeof templateConfig === "string") {
			(ko.components.defaultLoader as any).loadTemplate(name, templateConfig.replace(/[\t\n]/g, ''), callback);
		} else {
			(ko.components.defaultLoader as any).loadTemplate(name, templateConfig, callback);
		}
	}
}

export function AddComponentCleaner() {
	ko.components.loaders.unshift(componentLoader);
}
