export interface Result {
	Success: boolean;
	ErrorMessage: string;
}

export interface ResultOf<T> {
	Success: boolean;
	ErrorMessage: string;
	Data: T;
}
