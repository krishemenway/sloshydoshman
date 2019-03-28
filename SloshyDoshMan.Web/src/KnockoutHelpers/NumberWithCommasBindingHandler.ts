import * as ko from "knockout";

var Name : string = "NumberWithCommas";
export function NumberWithCommas(numberField: string) {
	return `${Name}: ${numberField}`;
}

function convertNumberToStringWithCommas(number: number) : string {
	let flooredNumber = Math.floor(number);
	let flooredNumberDifference = number - flooredNumber;
	let decimalAsString = flooredNumberDifference > 0 ? flooredNumberDifference.toString() : "";
	let formattedFlooredNumber = flooredNumber.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

	if(decimalAsString.length > 0) {
		return `${formattedFlooredNumber}.${decimalAsString}`;
	} else {
		return `${formattedFlooredNumber}`;
	}
}

function init(element: Element, valueAccessor: () => ko.Observable<number>) : void {
	ko.utils.setTextContent(element, convertNumberToStringWithCommas(ko.unwrap(valueAccessor())));
}

function update(element: Element, valueAccessor: () => ko.Observable<number>) : void {
	ko.utils.setTextContent(element, convertNumberToStringWithCommas(ko.unwrap(valueAccessor())));
}

ko.bindingHandlers[Name] = { init: init, update: update };
ko.virtualElements.allowedBindings[Name] = true;
