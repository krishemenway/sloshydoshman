import * as ko from "knockout";
import { Dictionary } from 'CommonDataStructures/Dictionary';

const observablesByHashField: Dictionary<ko.Observable<string>> = {};
const observableDefaultsByHashField: Dictionary<string> = {};

function parseQueryString(queryString: string) : Dictionary<string> {
	if(queryString[0] === "#") {
		queryString = queryString.slice(1);
	}

	let currentState : Dictionary<string> = {};

	queryString.split('&').forEach((keyValueString) => {
		let keyValueParts = keyValueString.split('=');

		let key = keyValueParts[0];
		let value = keyValueParts[1];

		if(!!key && !!value) {
			currentState[key] = value;
		}
	});

	return currentState;
}

function createQueryString(queryStringParams: Dictionary<string>) : string {
	return Object.keys(queryStringParams).map((key) => `${key}=${queryStringParams[key]}`).join('&');
}

export function SetState(newState: Dictionary<string>) : void {
	let queryString = createQueryString(newState);

	if (window.location.hash !== queryString) {
		window.location.hash = queryString;
	}
}

export function GetState() : Dictionary<string> {
	return parseQueryString(window.location.hash);
}

export function CreateHashChangeObservable(parameterName: string, defaultValue: string) {
	observableDefaultsByHashField[parameterName] = defaultValue;
	let observable = ko.observable<string>(GetState()[parameterName] || defaultValue);
	return observablesByHashField[parameterName] = observable;
}

$(window).on('hashchange', () => {
	let currentState = GetState();

	Object.keys(observablesByHashField).forEach((key) => {
		let observable = observablesByHashField[key];
		let currentValue = currentState[key];

		if (observable() !== currentValue) {
			if (currentValue === "" || currentValue === null || currentValue === undefined) {
				observable(observableDefaultsByHashField[key]);
			} else {
				observable(currentState[key])
			}
		}
	});
});
