import { CollaboraConnectionParams } from '@appTypes/collabora';

export interface Props extends CollaboraConnectionParams {
    csrfToken: string;
    text: {
        noScript: string;
    };
    className?: string;
}