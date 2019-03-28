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
	} else if (!params.Date) {
		throw `Missing Date parameter in momentFormatDate: ${params}`;
	}
}

function setText(element: Element, params: MomentFormatDateParams) {
	if (typeof params.Date === "string") {
		ko.utils.setTextContent(element, findOrCreateMoment(params.Date).format(params.Format));
	}
	else if (typeof params.Date === "function") {
		ko.utils.setTextContent(element, findOrCreateMoment(params.Date()).format(params.Format));
	}
}

function init(element: Element, valueAccessor: () => MomentFormatDateParams) {
	let params = valueAccessor();
	validateParams(params);
	setText(element, params);
}

function update(element: Element, valueAccessor: () => MomentFormatDateParams) {
	let params = valueAccessor();
	validateParams(params);
	setText(element, params);
}

ko.bindingHandlers[Name] = { init: init, update: update };
ko.virtualElements.allowedBindings[Name] = true;
