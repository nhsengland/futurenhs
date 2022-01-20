import { useRouter } from 'next/router';

import { routeParams } from '@constants/routes';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { BackLink } from '@components/BackLink';
import { Avatar } from '@components/Avatar';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';

import { Props } from './interfaces';  

/**
 * Group member template
 */
export const GroupMemberTemplate: (props: Props) => JSX.Element = ({
    user,
    text,
    member,
    actions,
    image
}) => {

    const router = useRouter();
    const backLinkHref: string = getRouteToParam({
        router: router,
        paramName: routeParams.MEMBERID
    });

    const { initials,
            firstName, 
            lastName, 
            pronouns, 
            email } = member ?? {};

    return (

        <GroupLayout 
            id="members"
            user={user}
            actions={actions}
            text={text}
            image={image} 
            className="u-bg-theme-3">
                <div className="c-page-body">
                    <LayoutColumn>
                        <BackLink 
                            href={backLinkHref}
                            text={{
                                link: "Back"
                            }} />
                    </LayoutColumn>
                    <LayoutColumnContainer justify="centre">
                        <LayoutColumn tablet={3} desktop={2}>
                            <Avatar image={null} initials={initials} />
                        </LayoutColumn>
                        <LayoutColumn tablet={7} desktop={8}>
                            <h2>My profile</h2>
                            <dl>
                                {firstName &&
                                    <>
                                        <dt className="u-text-bold">First name</dt>
                                        <dd>{firstName}</dd>
                                    </>
                                }
                                {lastName &&
                                    <>
                                        <dt className="u-text-bold">Last name</dt>
                                        <dd>{lastName}</dd>
                                    </>
                                }
                                {pronouns &&
                                    <>
                                        <dt className="u-text-bold">Preferred pronouns</dt>
                                        <dd>{pronouns}</dd>
                                    </>
                                }
                                {email &&
                                    <>
                                        <dt className="u-text-bold">Email address</dt>
                                        <dd>{email}</dd>
                                    </>
                                }
                            </dl>
                        </LayoutColumn>
                    </LayoutColumnContainer>
                </div>
        </GroupLayout>

    )

}