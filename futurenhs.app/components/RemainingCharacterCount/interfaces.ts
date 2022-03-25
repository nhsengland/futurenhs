export interface Props {
    id: string;
    currentCharacterCount: number;
    maxCharacterCount: number;
    remainingCharactersText: string,
    remainingCharactersExceededText: string,
    className?: string;
}