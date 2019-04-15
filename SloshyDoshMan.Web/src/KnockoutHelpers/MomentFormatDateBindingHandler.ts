import * as moment from "moment";
import { Observable } from "knockout";
import * as ko from "knockout"
import { Dictionary } from "CommonDataStructures/Dictionary";

var Name : string = "MomentFormatDate";
export function MomentFormat(dateParameterName: string, format: string) {
	return `${Name}: {Format: '${format}', Date: ${dateParameterName}}`;
}

interface MomentFormatDateParams {
	Date: Observable<string>|string;
	Format: string;
}

const existingMoments: Dictionary<moment.Moment> = {};
function findOrCreateMoment(date: string): moment.Moment {
	let existingMoment = existingMoments[date];

	if (existingMoment === undefined) {
		existingMoments[date] = moment(date);
	}

	return existingMoments[date];
}

function validateParams(params: MomentFormatDateParams) {
	if (!params.Format) {
		throw `Missing Format parameter in momentFormatDate: ${params}`;
	}

	if (!params.Date) {
		throw `Missing Date parameter in momentFormatDate: ${params}`;
	}
}

function getDateAsString(params: MomentFormatDateParams) {
	switch(typeof params.Date) {
		case "string":
			return params.Date;
		case "function":
			return params.Date();
		default:
			throw `Unknown type for moment parameter ${typeof params.Date}`;
	}
}

function init(_: Element, valueAccessor: () => MomentFormatDateParams) {
	validateParams(valueAccessor());
	return ko.bindingHandlers.text.init();
}

function update(element: Element, valueAccessor: () => MomentFormatDateParams) {
	let params = valueAccessor();
	validateParams(params);
	return ko.bindingHandlers.text.update(element, () => findOrCreateMoment(getDateAsString(params)).format(params.Format));
}

ko.bindingHandlers[Name] = { init: init, update: update };
ko.virtualElements.allowedBindings[Name] = true;
