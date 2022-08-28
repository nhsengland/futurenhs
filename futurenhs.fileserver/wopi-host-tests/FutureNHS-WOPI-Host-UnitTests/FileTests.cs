using FutureNHS.WOPIHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FutureNHS_WOPI_Host_UnitTests
{
    [TestClass]
    public sealed class FileTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void With_ThrowsIfFileNameIsNull()
        {
            _ = File.With(fileName: default, fileVersion: "file-version");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void With_ThrowsIfFileNameIsEmpty()
        {
            _ = File.With(fileName: string.Empty, fileVersion: "file-version");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void With_ThrowsIfFileNameIsWhiteSpace()
        {
            _ = File.With(fileName: " ", fileVersion: "file-version");
        }

        [TestMethod]
        public void With_DoesNotThrowIfFileVersionIsNull()
        {
            _ = File.With(fileName: "file-name", fileVersion: default);
        }

        [TestMethod]
        public void With_DoesNotThrowIfFileVersionIsEmpty()
        {
            _ = File.With(fileName: "file-name", fileVersion: string.Empty);
        }

        [TestMethod]
        public void With_DoesNotThrowIfFileVersionIsWhiteSpace()
        {
            _ = File.With(fileName: "file-name", fileVersion: " ");
        }

        [TestMethod]
        public void With_CorrectlyConstructsBasedOnParameters()
        {
            var fileName = "file-name";
            var fileVersion = "file-version";

            var file = File.With(fileName, fileVersion);

            Assert.IsFalse(file.IsEmpty);

            Assert.AreEqual(fileName, file.Name);
            Assert.AreEqual(fileVersion, file.Version);

            var id = string.Concat(fileName, "|", fileVersion);

            Assert.AreEqual(id, file.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void With_MinimumFileLengthIsEnforced()
        {
            var fileName = new string('x', File.FILENAME_MINIMUM_LENGTH - 1);

            _ = File.With(fileName, "file-version");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void With_MaximumFileLengthIsEnforced()
        {
            var fileName = new string('x', File.FILENAME_MAXIMUM_LENGTH + 1);

            _ = File.With(fileName, "file-version");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FromId_ThrowsIfFileVersionEncodedInIdMismatchesWithHeaderVersion()
        {
            var id = "file-name|file-version";

            var fileVersion = "file-version-2";

            _ = File.FromId(id, fileVersion);
        }

        [TestMethod]
        public void FromId_DoesNotThrowIfFileVersionEncodedInIdMatchesWithHeaderVersion()
        {
            var id = "file-name|file-version";

            var fileVersion = "file-version";

            _ = File.FromId(id, fileVersion);
        }

        [TestMethod]
        public void FromId_BuildsCorrectStructureFromEncodedIdWhenNoHeaderVersion()
        {
            var id = "file-name|file-version";

            var file = File.FromId(id, fileVersion: default);

            Assert.IsFalse(file.IsEmpty);

            Assert.AreEqual(id, file.Id);
            Assert.AreEqual("file-name", file.Name);
            Assert.AreEqual("file-version", file.Version);
        }

        [TestMethod]
        public void FromId_BuildsCorrectStructureFromEncodedIdWithHeaderVersion()
        {
            var id = "file-name|file-version";

            var file = File.FromId(id, fileVersion: "file-version");

            Assert.IsFalse(file.IsEmpty);

            Assert.AreEqual(id, file.Id);
            Assert.AreEqual("file-name", file.Name);
            Assert.AreEqual("file-version", file.Version);

            id = "file-name";

            file = File.FromId(id, fileVersion: "file-version");

            Assert.IsFalse(file.IsEmpty);

            Assert.AreEqual("file-name|file-version", file.Id);
            Assert.AreEqual("file-name", file.Name);
            Assert.AreEqual("file-version", file.Version);
        }



        [TestMethod]
        public void IsEmpty_ReturnsTrueForBlindlyConstructedInstance()
        {
            var file = new File();

            Assert.IsTrue(file.IsEmpty);
        }



        [TestMethod]
        public void Equals_IdentifiesWhenInstancesAreEqual()
        {
#pragma warning disable CS1718 // Comparison made to same variable
            var file1 = new File();
            var file2 = new File();

            Assert.IsTrue(file1.Equals(file1));
            Assert.IsTrue(file1.Equals(file2));
            Assert.IsTrue(file2.Equals(file1));
            Assert.IsTrue(file2.Equals(file2));

            Assert.IsTrue(file1 == file1);
            Assert.IsTrue(file1 == file2);
            Assert.IsTrue(file2 == file1);

            file1 = File.With("file-name", "file-version");
            file2 = File.With("file-name", "file-version");

            Assert.IsTrue(file1.Equals(file1));
            Assert.IsTrue(file1.Equals(file2));
            Assert.IsTrue(file2.Equals(file1));
            Assert.IsTrue(file2.Equals(file2));

            Assert.IsTrue(file1 == file1);
            Assert.IsTrue(file1 == file2);
            Assert.IsTrue(file2 == file1);

            var file1AsObj = (object)file1;
            var file2AsObj = (object)file2;

            Assert.IsTrue(file1AsObj.Equals(file1AsObj));
            Assert.IsTrue(file1AsObj.Equals(file1));
            Assert.IsTrue(file1AsObj.Equals(file2));
            Assert.IsTrue(file2AsObj.Equals(file1AsObj));
            Assert.IsTrue(file2AsObj.Equals(file1));
            Assert.IsTrue(file2AsObj.Equals(file2));
            Assert.IsTrue(file1.Equals(file1AsObj));
            Assert.IsTrue(file2.Equals(file1AsObj));
            Assert.IsTrue(file1AsObj.Equals(file2AsObj));
            Assert.IsTrue(file2.Equals(file1AsObj));
            Assert.IsTrue(file2.Equals(file2AsObj));

            // NB - File.Equals & Nullable<T>.Equals are actually object.Equals so no need to test them too

            Assert.IsTrue(file1AsObj == file1AsObj);
            Assert.IsFalse(file1AsObj == file2AsObj);               // Checks reference equality so will fail
            Assert.IsFalse(file2AsObj == file1AsObj);               // Checks reference equality so will fail    
            Assert.IsTrue(object.Equals(file1AsObj, file1AsObj));   // Checks value equality so should pass    
            Assert.IsTrue(object.Equals(file1AsObj, file2AsObj));   // Checks value equality so should pass   
            Assert.IsTrue(object.Equals(file2AsObj, file1AsObj));   // Checks value equality so should pass   

            var file1AsNullable = new File?(file1);
            var file2AsNullable = new File?(file2);

            Assert.IsTrue(file1AsNullable.Equals(file1));
            Assert.IsTrue(file1AsNullable.Equals(file1AsObj));
            Assert.IsTrue(file1.Equals(file1AsNullable));
            Assert.IsTrue(file1AsObj.Equals(file1AsNullable));
            Assert.IsTrue(file2AsNullable.Equals(file2));
            Assert.IsTrue(file2AsNullable.Equals(file2AsObj));
            Assert.IsTrue(file2.Equals(file2AsNullable));
            Assert.IsTrue(file2AsObj.Equals(file2AsNullable));
            Assert.IsTrue(file1AsNullable.Equals(file2AsNullable));

            Assert.IsTrue(file1AsNullable == file1AsNullable);
            Assert.IsTrue(file1AsNullable == file2AsNullable);
            Assert.IsTrue(file2AsNullable == file1AsNullable);
#pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void Equals_IdentifiesWhenInstancesAreNotEqual()
        {
            var file1 = File.With("file-name1", "file-version");
            var file2 = File.With("file-name2", "file-version");

            Assert.IsFalse(file1.Equals(new File()));
            Assert.IsFalse(file2.Equals(new File()));
            Assert.IsFalse(file1.Equals(file2));
            Assert.IsFalse(file2.Equals(file1));

            Assert.IsFalse(file1 == file2);
            Assert.IsFalse(file2 == file1);
            Assert.IsTrue(file1 != file2);
            Assert.IsTrue(file2 != file1);

            var file1AsObj = (object)file1;
            var file2AsObj = (object)file2;

            Assert.IsFalse(file1AsObj.Equals(file2));
            Assert.IsFalse(file2AsObj.Equals(file1AsObj));
            Assert.IsFalse(file2AsObj.Equals(file1));
            Assert.IsFalse(file2.Equals(file1AsObj));
            Assert.IsFalse(file1AsObj.Equals(file2AsObj));
            Assert.IsFalse(file2.Equals(file1AsObj));

            Assert.IsFalse(object.Equals(file2, file1AsObj));
            Assert.IsFalse(object.Equals(file1AsObj, file2AsObj));
            Assert.IsFalse(object.Equals(file2, file1AsObj));

            Assert.IsFalse(file1AsObj == file2AsObj);
            Assert.IsTrue(file1AsObj != file2AsObj);
            Assert.IsTrue(file2AsObj != file1AsObj);

            var file1AsNullable = new File?(file1);
            var file2AsNullable = new File?(file2);

            Assert.IsFalse(file1AsNullable.Equals(file2AsNullable));
            Assert.IsFalse(file1AsNullable.Equals(file2AsObj));
            Assert.IsFalse(file1AsNullable.Equals(file2));
            Assert.IsFalse(file2AsNullable.Equals(file1AsNullable));
            Assert.IsFalse(file2AsNullable.Equals(file1AsObj));
            Assert.IsFalse(file2AsNullable.Equals(file1));

            Assert.IsFalse(file1.Equals(new File?()));
            Assert.IsFalse(file2.Equals(new File?()));
            Assert.IsFalse(file1AsObj.Equals(new File?()));
            Assert.IsFalse(file2AsObj.Equals(new File?()));
            Assert.IsFalse(file1AsNullable.Equals(new File?()));
            Assert.IsFalse(file2AsNullable.Equals(new File?()));

            Assert.IsTrue(file1AsNullable != file2AsNullable);
            Assert.IsTrue(file2AsNullable != file1AsNullable);
        }



        [TestMethod]
        public void GetHashCode_InstancesConstructedTheSameHaveIdenticalHashCodes()
        {
            var file1 = File.With("file-name", "file-version");
            var file2 = File.With("file-name", "file-version");

            var file1HashCode = file1.GetHashCode();
            var file2HashCode = file2.GetHashCode();

            Assert.AreEqual(file1HashCode, file2HashCode);

            file1 = new File();
            file2 = new File();

            file1HashCode = file1.GetHashCode();
            file2HashCode = file2.GetHashCode();

            Assert.AreEqual(file1HashCode, file2HashCode);
        }

        [TestMethod]
        public void GetHashCode_InstancesNotConstructedTheSameHaveDifferentHashCodes()
        {
            var file1 = File.With("file-name1", "file-version");
            var file2 = File.With("file-name2", "file-version");

            var file1HashCode = file1.GetHashCode();
            var file2HashCode = file2.GetHashCode();

            Assert.AreNotEqual(file1HashCode, file2HashCode);
        }



        [TestMethod]
        public void ImplictConvertToString_ReturnsNullWhenIsEmpty()
        {
            var empty = new File();

            Assert.IsTrue(empty.IsEmpty);

            var emptyAsString = (string)empty;
            
            Assert.IsNull(emptyAsString);
        }

        [TestMethod]
        public void ImplicitConvertToString_ReturnsIdWhenNotEmpty()
        {
            var notEmpty = File.With("file-name", "file-version");

            var notEmptyAsString = (string)notEmpty;

            var id = notEmpty.Id;

            Assert.AreEqual(id, notEmptyAsString);
        }

        [TestMethod]
        public void ImplicitConvertFromString_ReturnsEmptyWhenSourceIsNull()
        {
            var str = default(string);

            var file = (File)str;

            Assert.IsTrue(file.IsEmpty);
        }

        [TestMethod]
        public void ImplicitConvertFromString_ReturnsEmptyWhenSourceIsEmpty()
        {
            var file = (File)string.Empty;

            Assert.IsTrue(file.IsEmpty);
        }

        [TestMethod]
        public void ImplicitConvertFromString_ReturnsEmptyWhenSourceNotParsable()
        {
            var str = "";

            var file = (File)str;

            Assert.IsTrue(file.IsEmpty);
        }

        [TestMethod]
        public void ImplicitConvertFromString_ReturnsCorrectlyConstructedInstantWhenPresentedWithParsableStructure()
        {
            var str = "file-name|file-version";

            var file = (File)str;

            Assert.IsFalse(file.IsEmpty);

            Assert.AreEqual(File.With("file-name", "file-version"), file);
        }
    }
}
