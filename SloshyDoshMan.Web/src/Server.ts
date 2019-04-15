import {Dictionary} from './CommonDataStructures/Dictionary';

export interface RecentGameResponse {
	TotalGames: number;
	RecentGames: PlayedGame[];
}

export interface GameViewModel {
	Scoreboard: Scoreboard;
	PlayedGame: PlayedGame
}

export interface Scoreboard {
	Players: ScoreboardPlayer[];
	TotalKills: number;
}

export interface ScoreboardPlayer {
	PlayerWaveInfo: Dictionary<PlayerWaveInfo>;
	SteamId: string;
	UserName: string;
}

export interface PlayerWaveInfo {
	Wave: number;
	Perk: string;
	Kills: number;
}

export interface Player {
	Name : string;
	SteamId : number;
}

export interface PlayerPerkStatistic {
	Perk: string;
	TotalWavesPlayed: number;
	TotalKills: number;
}

export interface PlayerMapStatistic {
	Map: string;
	Difficulty: string;
	GamesPlayed: number;
	GamesWon: number;
	TotalKills: number;
	FarthestWave: number;
}

export interface PlayerViewModel {
	Player: Player;
	TotalKills: number;
	TotalGames: number;
	AllGames: PlayedGame[];
	MapStatistics: PlayerMapStatistic[];
	PerkStatistics: PlayerPerkStatistic[];
}

export interface PlayedGame {
	PlayedGameId: string;

	Map: string;
	Difficulty: string;

	Length: string;
	ReachedWave: number;
	TotalWaves: number;

	TimeStarted: string;
	TimeFinished: string;

	PlayersWon: boolean;
}
