export type UserStatus = 'Member' | 'LegacyMember' | 'Invited' | 'Uninvited'

export interface User {
    id: string;
    status: UserStatus;
    text: {
        userName: string
    }
    image?: Image
}
