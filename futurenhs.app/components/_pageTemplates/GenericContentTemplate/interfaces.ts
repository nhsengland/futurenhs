import { Page } from '@appTypes/page'
import { User } from '@appTypes/user'

export interface Props extends Page {
    user: User
    pageTitle?: string
}
