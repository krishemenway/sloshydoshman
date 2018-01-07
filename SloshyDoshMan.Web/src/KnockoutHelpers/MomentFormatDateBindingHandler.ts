import * as moment from 'moment';

export var Name : string = "MomentFormatDate";

interface MomentFormatDateParams {
	Date: KnockoutObservable<string>|string;
	Format: string;
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
		ko.utils.setTextContent(element, moment(params.Date).format(params.Format));
	}
	else if (typeof params.Date === "function") {
		ko.utils.setTextContent(element, moment(params.Date()).format(params.Format));
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