import { CollaboraConnectionParams } from '@appTypes/collabora'

export interface Props extends CollaboraConnectionParams {
    csrfToken: string
    className?: string
}
