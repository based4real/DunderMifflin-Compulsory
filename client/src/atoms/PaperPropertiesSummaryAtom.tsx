import { atom } from "jotai";
import { PaperPropertySummaryViewModel } from "../Api";

export const PaperPropertiesSummaryAtom = atom<PaperPropertySummaryViewModel[]>([]);