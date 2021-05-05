using FarseerPhysics.Dynamics.Contacts;

namespace GameProject.Ecs.Physics
{
    internal delegate bool CollisionEventHandler(Collider collider1, Collider collider2, Contact contact);
}