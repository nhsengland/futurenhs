import { GenericPageTextContent } from '@appTypes/content';

interface Text extends GenericPageTextContent {}

export interface Props {
    isAuthenticated: boolean;
    text: Text;
}