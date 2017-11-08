import {Dictionary} from 'CommonDataStructures/Dictionary';
import * as ko from "knockout";

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

function findValueForKey(key: string) : string {
	let state = parseQueryString(window.location.hash);
	return state[key];
}

function setValueForKey(key: string, value: string) : void {
	let currentState = parseQueryString(window.location.hash);

	if(!value) {
		delete currentState[key];
	} else {
		currentState[key] = value;
	}

	SetState(currentState);
}

function setValueToTargetForKey(targetObservable : KnockoutObservable<any>, hashKey: string) {
	let value : any = findValueForKey(hashKey);
	let valueAsNumber = parseInt(value, 10);

	if(!isNaN(valueAsNumber) && valueAsNumber.toString() === value) {
		value = valueAsNumber;
	}

	if(value != targetObservable()) {
		targetObservable(value);
	}
}

function initialize(targetObservable : KnockoutObservable<string>, key: string) {
	setValueToTargetForKey(targetObservable, key);

	targetObservable.subscribe((newValue) => {
		let currentValue = findValueForKey(key);

		if(!!currentValue && currentValue == newValue) {
			return;
		}

		setValueForKey(key, newValue);
	});

	$(window).on('hashchange', () => {
		setValueToTargetForKey(targetObservable, key);
	});
}

export function SetState(newState: Dictionary<string>) : void {
	window.location.hash = createQueryString(newState);
}

export function CreateObservable<T>(parameterName: string, defaultValue: T) {
	return ko.observable(defaultValue).extend({hashchange: parameterName});
}

(<any>ko.extenders).hashchange = initialize;
